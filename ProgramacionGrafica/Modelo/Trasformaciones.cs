using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace ProgramacionGrafica.Modelo
{
    public abstract class Transformable
    {
        private Vector3 _posicion = Vector3.Zero;
        private Vector3 _rotacion = Vector3.Zero;
        private Vector3 _escala = Vector3.One;

        public Vector3 Posicion
        {
            get => _posicion;
            protected set => _posicion = value;
        }

        public Vector3 Rotacion
        {
            get => _rotacion;
            protected set => _rotacion = value;
        }

        public Vector3 Escala
        {
            get => _escala;
            protected set => _escala = value;
        }

        // Métodos de traslación
        public void Trasladar(Vector3 desplazamiento) => _posicion += desplazamiento;
        public void EstablecerPosicion(Vector3 nuevaPos) => _posicion = nuevaPos;

        // Métodos de rotación (corregidos)
        public void Rotar(Vector3 angulos) => _rotacion += angulos;
        public void RotarX(float angulo) => _rotacion.X += angulo;
        public void RotarY(float angulo) => _rotacion.Y += angulo;
        public void RotarZ(float angulo) => _rotacion.Z += angulo;
        public void EstablecerRotacion(Vector3 nuevaRot) => _rotacion = nuevaRot;

        // Métodos de escalado
        public void Escalar(Vector3 factores) => _escala *= factores;
        public void EstablecerEscala(Vector3 nuevaEscala) => _escala = nuevaEscala;

        public Matrix4 ObtenerMatrizTransformacion()
        {
            return Matrix4.CreateScale(_escala) *
                   Matrix4.CreateRotationX(_rotacion.X) *
                   Matrix4.CreateRotationY(_rotacion.Y) *
                   Matrix4.CreateRotationZ(_rotacion.Z) *
                   Matrix4.CreateTranslation(_posicion);
        }
    }
}
