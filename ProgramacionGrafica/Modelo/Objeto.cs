using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ProgramacionGrafica.Modelo
{
    public class Objeto : IDisposable
    {
        private bool _disposed = false;
        public string Nombre { get; set; }
        public List<Parte> Partes { get; } = new List<Parte>();

        public Vector3 Posicion { get; set; } = Vector3.Zero;
        public Vector3 Rotacion { get; set; } = Vector3.Zero;
        public Vector3 Escala { get; set; } = Vector3.One;


        #region transformaciones

        public void Trasladar(float x, float y, float z) => Posicion += new Vector3(x, y, z);
        public void Trasladar(Vector3 desplazamiento) => Posicion += desplazamiento;
        public void EstablecerPosicion(float x, float y, float z) => Posicion = new Vector3(x, y, z);

 
        public void Rotar(float anguloX, float anguloY, float anguloZ) => Rotacion += new Vector3(anguloX, anguloY, anguloZ);
        public void EstablecerRotacion(float x, float y, float z) => Rotacion = new Vector3(x, y, z);

        public void Escalar(float factor) => Escala *= new Vector3(factor);
        public void Escalar(float x, float y, float z) => Escala *= new Vector3(x, y, z);
        public void EstablecerEscala(float x, float y, float z) => Escala = new Vector3(x, y, z);


        public void RotarAlrededorDe(Vector3 puntoCentral, Vector3 angulos)
        {
            Matrix4 transform = Matrix4.CreateTranslation(-puntoCentral) *
                               Matrix4.CreateRotationX(angulos.X) *
                               Matrix4.CreateRotationY(angulos.Y) *
                               Matrix4.CreateRotationZ(angulos.Z) *
                               Matrix4.CreateTranslation(puntoCentral);

            Posicion = Vector3.TransformPosition(Posicion, transform);
            Rotacion += angulos;
        }

        #endregion

        public void AgregarParte(Parte parte)
        {
            if (parte == null)
                throw new ArgumentNullException(nameof(parte));

            Partes.Add(parte);
        }

        
        public Parte CrearParte(List<Cara> caras)
        {
            var nuevaParte = new Parte();
            foreach (var cara in caras)
            {
                nuevaParte.AgregarCara(cara);
            }
            Partes.Add(nuevaParte);
            return nuevaParte;
        }

        public Matrix4 ObtenerMatrizTransformacion()
        {
            return Matrix4.CreateScale(Escala) *
                   Matrix4.CreateRotationX(Rotacion.X) *
                   Matrix4.CreateRotationY(Rotacion.Y) *
                   Matrix4.CreateRotationZ(Rotacion.Z) *
                   Matrix4.CreateTranslation(Posicion);
        }

        public void Dibujar(Matrix4 modeloPadre, Matrix4 view, Matrix4 projection, int shaderProgram)
        {
            Matrix4 modeloObjeto = modeloPadre * ObtenerMatrizTransformacion();

            foreach (var parte in Partes)
            {
                parte.Dibujar(modeloObjeto, view, projection, shaderProgram);
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
                    foreach (var parte in Partes)
                    {
                        parte?.Dispose();
                    }
                    Partes.Clear();
                }

                _disposed = true;
            }
        }

        ~Objeto()
        {
            Dispose(false);
        }
    }
}