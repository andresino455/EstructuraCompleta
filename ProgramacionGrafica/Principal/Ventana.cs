using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ProgramacionGrafica.Modelo;

namespace ProgramacionGrafica.Principal
{
    public class Ventana : GameWindow
    {
        private int _shaderProgram;
        private Escenario _escenario;
        private Matrix4 _projection;
        private float _anguloRotacion = 0.0f;

        private Camara _camara;
        private Vector2 _ultimaPosRaton;
        private bool _primerMovimiento = true;

        public Ventana() : base(GameWindowSettings.Default,
            new NativeWindowSettings() { Size = new Vector2i(800, 600), Title = "OpenTK + Cámara Libre" })
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            _shaderProgram = CrearShaderProgram();

            Vector3 posicionInicial = new Vector3(3, 3, 3); // O cualquier punto desde el que quieras mirar
            Vector3 objetivo = Vector3.Zero; // El cubo está en el origen
            Vector3 up = Vector3.UnitY; // Dirección "arriba" del mundo

            _camara = new Camara(posicionInicial, objetivo, up);
            _camara.Position = new Vector3(0, 0, 3);
            _camara.Front = Vector3.Normalize(new Vector3(0, 0, 0) - _camara.Position);

            CursorGrabbed = true;

            _projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f),
                Size.X / (float)Size.Y, 0.1f, 100f);

            _escenario = new Escenario();
            _escenario.CargarContenido();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
                Close();

            Vector2 movimiento = Vector2.Zero;
            float vertical = 0;

            if (input.IsKeyDown(Keys.W)) movimiento.Y += 1;
            if (input.IsKeyDown(Keys.S)) movimiento.Y -= 1;
            if (input.IsKeyDown(Keys.A)) movimiento.X -= 1;
            if (input.IsKeyDown(Keys.D)) movimiento.X += 1;
            if (input.IsKeyDown(Keys.Q)) vertical += 1;
            if (input.IsKeyDown(Keys.E)) vertical -= 1;

            _camara.ProcessKeyboard(movimiento, vertical, (float)e.Time);

            // Movimiento del ratón
            var mouse = MouseState;
            if (_primerMovimiento)
            {
                _ultimaPosRaton = new Vector2(mouse.X, mouse.Y);
                _primerMovimiento = false;
            }
            else
            {
                var deltaX = mouse.X - _ultimaPosRaton.X;
                var deltaY = mouse.Y - _ultimaPosRaton.Y;
                _ultimaPosRaton = new Vector2(mouse.X, mouse.Y);

                _camara.ProcessMouse(deltaX, deltaY);
            }

            _anguloRotacion += (float)e.Time * MathHelper.DegreesToRadians(60f);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(_shaderProgram);



            //foreach (var objeto in _escenario.Objetos)
            //{
            //    objeto.Rotacion = new Vector3(0, _anguloRotacion, 0);
            //}

            Matrix4 view = _camara.GetViewMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);


            _escenario.Objetos[0].Posicion = new Vector3(-1.5f, 3f, 0f); 
            _escenario.Objetos[1].Posicion = new Vector3(1.5f, 0f, 0f);   

            _escenario.Dibujar(view, projection, _shaderProgram);

            SwapBuffers();
        }

        private int CrearShaderProgram()
        {
            string vertexShaderSource = @"#version 330 core
                layout(location = 0) in vec3 aPos;
                layout(location = 1) in vec3 aNormal;
                
                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;
                
                out vec3 Normal;
                
                void main()
                {
                    gl_Position = projection * view * model * vec4(aPos, 1.0);
                    Normal = mat3(transpose(inverse(model))) * aNormal;
                }";

            string fragmentShaderSource = @"#version 330 core
                out vec4 FragColor;
                
                in vec3 Normal;
                
                void main()
                {
                    FragColor = vec4(normalize(Normal) * 0.5 + 0.5, 1.0);
                }";

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);

            int shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);

            GL.DetachShader(shaderProgram, vertexShader);
            GL.DetachShader(shaderProgram, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return shaderProgram;
        }

        protected override void OnUnload()
        {
            GL.DeleteProgram(_shaderProgram);
            base.OnUnload();
        }
    }
}
