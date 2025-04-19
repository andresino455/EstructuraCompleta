using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ProgramacionGrafica.Modelo
{
    public class Escenario : IDisposable 
    {
        private bool _disposed = false;
        public List<Objeto> Objetos { get; } = new List<Objeto>();


        private Vector3 _posicion = Vector3.Zero;
        private Vector3 _rotacion = Vector3.Zero;
        private Vector3 _escala = Vector3.One;

        public Vector3 Posicion
        {
            get => _posicion;
            set => _posicion = value;
        }

        public Vector3 Rotacion
        {
            get => _rotacion;
            set => _rotacion = value;
        }

        public Vector3 Escala
        {
            get => _escala;
            set => _escala = value;
        }

        #region Trasformaciones
        public void Trasladar(Vector3 desplazamiento) {
            _posicion += desplazamiento;

        }

        public void FijarTraslacion(Vector3 desplazamiento)
        {
            _posicion = desplazamiento;

        }

        public void Rotar(Vector3 angulos)
        {
            _rotacion.X += angulos.X;
            _rotacion.Y += angulos.Y;
            _rotacion.Z += angulos.Z;
        }

        public void FijarRotacion(Vector3 angulos)
        {
            _rotacion.X = angulos.X;
            _rotacion.Y = angulos.Y;
            _rotacion.Z = angulos.Z;
        }

        public void RotarObjetoPorNombre(string nombreObjeto, Vector3 angulos)
        {
            var objeto = Objetos.FirstOrDefault(o => o.Nombre == nombreObjeto);
            if (objeto != null)
            {
                objeto.Rotar(angulos.X,angulos.Y,angulos.Z);
            }
            else
            {
                Console.WriteLine($"Advertencia: Objeto '{nombreObjeto}' no encontrado.");
            }
        }

        public void EscalarObjetoPorNombre(string nombreObjeto, Vector3 angulos)
        {
            var objeto = Objetos.FirstOrDefault(o => o.Nombre == nombreObjeto);
            if (objeto != null)
            {
                objeto.EstablecerEscala(angulos.X, angulos.Y, angulos.Z);
            }
            else
            {
                Console.WriteLine($"Advertencia: Objeto '{nombreObjeto}' no encontrado.");
            }
        }

        public void TrasladarObjetoPorNombre(string nombreObjeto, Vector3 angulos)
        {
            var objeto = Objetos.FirstOrDefault(o => o.Nombre == nombreObjeto);
            if (objeto != null)
            {
                objeto.Trasladar(angulos.X, angulos.Y, angulos.Z);
            }
            else
            {
                Console.WriteLine($"Advertencia: Objeto '{nombreObjeto}' no encontrado.");
            }
        }

        public void PosicionarObjetoPorNombre(string nombreObjeto, Vector3 angulos)
        {
            var objeto = Objetos.FirstOrDefault(o => o.Nombre == nombreObjeto);
            if (objeto != null)
            {
                objeto.EstablecerPosicion(angulos.X, angulos.Y, angulos.Z);
            }
            else
            {
                Console.WriteLine($"Advertencia: Objeto '{nombreObjeto}' no encontrado.");
            }
        }

        public void Escalar(Vector3 factores) {
            Escala *= factores;
        }
        public void FijarEscala(Vector3 factores)
        {
            Escala = factores;
        }

        public Matrix4 ObtenerMatrizTransformacion()
        {
            return Matrix4.CreateScale(_escala) *
                   Matrix4.CreateRotationX(_rotacion.X) *
                   Matrix4.CreateRotationY(_rotacion.Y) *
                   Matrix4.CreateRotationZ(_rotacion.Z) *
                   Matrix4.CreateTranslation(_posicion);
        }

        #endregion

        public void CargarContenido()
        {

            var objeto1 = CrearObjeto("LetraU1", "Objetos/LetraU.json");
            Objetos.Add(objeto1);

            var objeto2 = CrearObjeto("LetraU2", "Objetos/LetraU.json");
            Objetos.Add(objeto2);

            ConstruirBuffers();
        }

        public Objeto CrearObjeto(string nombre, string rutaArchivo)
        {
            var objeto = new Objeto { Nombre = nombre };
            var parte = new Parte();
            var cara = new Cara();

            var (vertices, indices) = CargarVerticesDesdeJson(rutaArchivo);

            cara.Vertices.AddRange(vertices);
            cara.Indices.AddRange(indices);

            parte.AgregarCara(cara);
            objeto.AgregarParte(parte);

            return objeto;
        }


        public void ConstruirBuffers()
        {
            foreach (var objeto in Objetos)
            {
                foreach (var parte in objeto.Partes)
                {
                    foreach (var cara in parte.Caras)
                    {

                        var vertexData = new float[cara.Vertices.Count * 8]; 

                        for (int i = 0; i < cara.Vertices.Count; i++)
                        {
                            var v = cara.Vertices[i];
                            vertexData[i * 8 + 0] = v.Posicion.X;
                            vertexData[i * 8 + 1] = v.Posicion.Y;
                            vertexData[i * 8 + 2] = v.Posicion.Z;
                            vertexData[i * 8 + 3] = v.Normal.X;
                            vertexData[i * 8 + 4] = v.Normal.Y;
                            vertexData[i * 8 + 5] = v.Normal.Z;
                            vertexData[i * 8 + 6] = v.TexCoord.X;
                            vertexData[i * 8 + 7] = v.TexCoord.Y;
                        }

             
                        cara.VAO = GL.GenVertexArray();
                        GL.BindVertexArray(cara.VAO);

                    
                        cara.VBO = GL.GenBuffer();
                        GL.BindBuffer(BufferTarget.ArrayBuffer, cara.VBO);
                        GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

                     
                        cara.EBO = GL.GenBuffer();
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, cara.EBO);
                        GL.BufferData(BufferTarget.ElementArrayBuffer, cara.Indices.Count * sizeof(uint), cara.Indices.ToArray(), BufferUsageHint.StaticDraw);

                        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
                        GL.EnableVertexAttribArray(0);

                        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
                        GL.EnableVertexAttribArray(1);

                        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
                        GL.EnableVertexAttribArray(2);

                        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                        GL.BindVertexArray(0); 
                    }
                }
            }
        }


        public void Dibujar(Matrix4 view, Matrix4 projection, int shaderProgram)
        {
            Matrix4 modeloEscenario = ObtenerMatrizTransformacion();

            foreach (var objeto in Objetos)
            {
                objeto.Dibujar(modeloEscenario, view, projection, shaderProgram);
            }
        }


        public class VerticeJson
        {
            public float[] Posicion { get; set; }    // [x, y, z]
            public float[] Normal { get; set; }      // [nx, ny, nz]
            public float[] TexCoord { get; set; }    // [u, v]
        }

        public class ModeloJson
        {
            public List<VerticeJson> Vertices { get; set; }
            public List<int> Indices { get; set; }
        }

        public (List<Vertice>, List<int>) CargarVerticesDesdeJson(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
                throw new FileNotFoundException($"Archivo JSON no encontrado: {rutaArchivo}");

            // Configurar opciones de deserialización
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            // Leer y deserializar el archivo
            string json = File.ReadAllText(rutaArchivo);
            var modeloJson = JsonSerializer.Deserialize<ModeloJson>(json, options);

            // Convertir a objetos Vertice
            var vertices = new List<Vertice>();
            foreach (var v in modeloJson.Vertices)
            {
                vertices.Add(new Vertice(
                    v.Posicion[0], v.Posicion[1], v.Posicion[2],
                    v.Normal[0], v.Normal[1], v.Normal[2],
                    v.TexCoord[0], v.TexCoord[1]
                ));
            }

            // Devolver tanto vértices como índices
            return (vertices, modeloJson.Indices);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var objeto in Objetos)
                    {
                        objeto?.Dispose();
                    }
                    Objetos.Clear();
                }
                _disposed = true;
            }
        }
    }
}