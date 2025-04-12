using OpenTK.Mathematics;

namespace ProgramacionGrafica.Modelo
{
    public class Vertice
    {
        public Vector3 Posicion { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 TexCoord { get; set; }
        public int Indice { get; set; }


        public Vertice(float x, float y, float z)
        {
            Posicion = new Vector3(x, y, z);
            Normal = Vector3.Zero;
            TexCoord = Vector2.Zero;
            Indice = -1;
        }


        public Vertice(float x, float y, float z,
                      float nx, float ny, float nz,
                      float u, float v)
        {
            Posicion = new Vector3(x, y, z);
            Normal = new Vector3(nx, ny, nz);
            TexCoord = new Vector2(u, v);
            Indice = -1;
        }

        public Vertice(Vector3 posicion, Vector3 normal, Vector2 texCoord)
        {
            Posicion = posicion;
            Normal = normal;
            TexCoord = texCoord;
            Indice = -1;
        }
    }
}