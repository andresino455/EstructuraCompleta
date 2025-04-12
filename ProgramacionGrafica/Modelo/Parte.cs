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

        public void Dibujar(Matrix4 transformacionPadre, Matrix4 view, Matrix4 projection, int shaderProgram)
        {
            Matrix4 transformacionFinal = transformacionPadre * _transformacionLocal;

            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "model"), false, ref transformacionFinal);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "view"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "projection"), false, ref projection);


                GL.UseProgram(shaderProgram);
                foreach (var cara in Caras)
                {
                    GL.BindVertexArray(cara.VAO);
                    GL.DrawElements(PrimitiveType.Triangles, cara.Indices.Count, DrawElementsType.UnsignedInt, 0);
                    GL.BindVertexArray(0);
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
