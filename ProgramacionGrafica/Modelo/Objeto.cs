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

        public Matrix4 ObtenerTransformacionGlobal()
        {
            return Matrix4.CreateScale(Escala) *
                   Matrix4.CreateRotationX(Rotacion.X) *
                   Matrix4.CreateRotationY(Rotacion.Y) *
                   Matrix4.CreateRotationZ(Rotacion.Z) *
                   Matrix4.CreateTranslation(Posicion);
        }

        public void Dibujar(Matrix4 view, Matrix4 projection, int shaderProgram)
        {
            Matrix4 transformacionGlobal = ObtenerTransformacionGlobal();

            foreach (var parte in Partes)
            {
                parte.Dibujar(transformacionGlobal, view, projection, shaderProgram);
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