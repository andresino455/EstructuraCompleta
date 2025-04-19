using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ProgramacionGrafica.Modelo
{
    public class Parte : IDisposable
    {
        private bool _disposed = false;
        public List<Cara> Caras { get; } = new List<Cara>();

        private Vector3 _posicion = Vector3.Zero;
        private Vector3 _rotacion = Vector3.Zero;
        private Vector3 _escala = Vector3.One;

        // Propiedades
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

        // Métodos de transformación
        public void Trasladar(Vector3 desplazamiento) => _posicion += desplazamiento;

        public void Rotar(Vector3 angulos)
        {
            _rotacion.X += angulos.X;
            _rotacion.Y += angulos.Y;
            _rotacion.Z += angulos.Z;
        }

        public void Escalar(Vector3 factores) => _escala *= factores;

        public Matrix4 ObtenerMatrizTransformacion()
        {
            return Matrix4.CreateScale(_escala) *
                   Matrix4.CreateRotationX(_rotacion.X) *
                   Matrix4.CreateRotationY(_rotacion.Y) *
                   Matrix4.CreateRotationZ(_rotacion.Z) *
                   Matrix4.CreateTranslation(_posicion);
        }

        private Matrix4 _transformacionLocal = Matrix4.Identity;

        public Matrix4 TransformacionLocal
        {
            get => _transformacionLocal;
            set => _transformacionLocal = value;
        }

        public void AgregarCara(Cara cara)
        {
            if (cara == null)
                throw new ArgumentNullException(nameof(cara));

            Caras.Add(cara);
        }

        public void CrearCara(List<Vertice> vertices, List<int> indices)
        {
            var nuevaCara = new Cara();
            nuevaCara.Vertices.AddRange(vertices);
            nuevaCara.Indices.AddRange(indices);
            nuevaCara.ConstruirBuffers();
            Caras.Add(nuevaCara);
        }

        public void Dibujar(Matrix4 modeloPadre, Matrix4 view, Matrix4 projection, int shaderProgram)
        {
            Matrix4 modeloFinal = modeloPadre * ObtenerMatrizTransformacion();

            // Pasa matrices UNICAMENTE aquí (evita repetir en Cara)
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "model"), false, ref modeloFinal);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "view"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "projection"), false, ref projection);

            foreach (var cara in Caras)
            {
                cara.Dibujar(shaderProgram); 
            }
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
                    // Liberar recursos administrados
                    foreach (var cara in Caras)
                    {
                        cara?.Dispose();
                    }
                    Caras.Clear();
                }

                _disposed = true;
            }
        }

        ~Parte()
        {
            Dispose(false);
        }
    }
}
