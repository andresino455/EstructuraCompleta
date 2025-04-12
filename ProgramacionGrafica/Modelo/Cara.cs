using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ProgramacionGrafica.Modelo
{
    public class Cara : IDisposable
    {
        private int _vao, _vbo, _ebo;
        public List<Vertice> Vertices { get; } = new List<Vertice>();
        public List<int> Indices { get; } = new List<int>();

        public int VAO { get; set; }
        public int VBO { get; set; }
        public int EBO { get; set; }

        public void CargarDesdeJson(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
                throw new FileNotFoundException($"Archivo no encontrado: {rutaArchivo}");

            string json = File.ReadAllText(rutaArchivo);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<JsonData>(json, options);

            Vertices.Clear();
            for (int i = 0; i < data.Vertices.Length; i += 3)
            {
                Vertices.Add(new Vertice(
                    data.Vertices[i],
                    data.Vertices[i + 1],
                    data.Vertices[i + 2]
                ));
            }

            // Cargar índices
            Indices.Clear();
            Indices.AddRange(data.Indices);
        }

        public void ConstruirBuffers()
        {

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            var vertexData = new float[Vertices.Count * 8]; 
            for (int i = 0; i < Vertices.Count; i++)
            {
                var v = Vertices[i];

                vertexData[i * 8] = v.Posicion.X;
                vertexData[i * 8 + 1] = v.Posicion.Y;
                vertexData[i * 8 + 2] = v.Posicion.Z;

                vertexData[i * 8 + 3] = v.Normal.X;
                vertexData[i * 8 + 4] = v.Normal.Y;
                vertexData[i * 8 + 5] = v.Normal.Z;

                vertexData[i * 8 + 6] = v.TexCoord.X;
                vertexData[i * 8 + 7] = v.TexCoord.Y;
            }


            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);


            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Count * sizeof(int), Indices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);


            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);


            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        public void Dibujar()
        {
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
            GL.DeleteVertexArray(VAO);
            GC.SuppressFinalize(this);
        }


        private class JsonData
        {
            public float[] Vertices { get; set; }
            public int[] Indices { get; set; }
            public float[] Normals { get; set; }
            public float[] TexCoords { get; set; }
        }
    }
}
