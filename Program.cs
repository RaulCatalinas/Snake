using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Snake
{
    internal class Program
    {
        // Contiene las 4 direcciones de la serpiente

        enum Direccion
        {
            Arriba,
            Abajo,
            Izquierda,
            Derecha
        }

        // Para las coordenadas de la serpiente

        class Punto
        {
            //Constructor básico para almacenar las coordenadas x e y

            public Punto(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            //Constructor vació para la comida

            public Punto() { }

            public int X { get; set; }
            public int Y { get; set; }
        }

        // Condición para que se repita el bucle principal del juego

        static bool Juega = true;

        // Dirección inicial de la serpiente

        static Direccion dir = Direccion.Izquierda;

        // Lista para los puntos dela serpiente

        static List<Punto> Serpiente = new List<Punto>();

        // Coordenadas x e y para la comida

        static Punto posicionComida = new Punto();

        // Para evitar problemas con el movimiento

        static bool canMove = true;

        // Para la puntuación

        static int Score = 0;

        // Para la puntuación mas alta

        static int ScoreMaximo = 0;

        // Para el nombre de la persona que ha conseguido la puntuación mas alta

        static string Nombre = null;

        /* Al ser el método estático las variables también tienen que ser estáticas para poder acceder a ellas */

        static void Main(string[] args)
        {
            //Poner un titulo a la consola de windows

            Console.Title = "Snake by Raul Catalinas Esteban (Hecho en C#)";

            //Leer la máxima puntuación conseguida

            LeerScoreMaximo();

            //Iniciar menú

            Menu();
        }

        // Funciones

        private static void Mover()
        {
            //Iniciamos la posicion auxiliar en null

            Punto posAux = null;

            //Iniciamos canMove en false

            canMove = false;

            //Recorrer todos los puntos que componen el cuerpo del serpiente

            for (var i = 0; i < Serpiente.Count; i++)
            {
                //Situar el cursor en la posicion de la serpiente

                Console.SetCursorPosition(Serpiente[i].X, Serpiente[i].Y);

                //Borrar el cuerpo de la serpiente

                Console.Write(" ");

                //Si la longitud de la serpiente es igual a 1

                if (i == 0)
                {
                    //Almacenar posicion

                    posAux = new Punto(Serpiente[0].X, Serpiente[0].Y);

                    //Mover serpiente

                    if (dir == Direccion.Arriba)
                    {
                        Serpiente[i].Y -= 1;
                    }
                    else if (dir == Direccion.Abajo)
                    {
                        Serpiente[i].Y += 1;
                    }
                    else if (dir == Direccion.Izquierda)
                    {
                        Serpiente[i].X -= 1;
                    }
                    else if (dir == Direccion.Derecha)
                    {
                        Serpiente[i].X += 1;
                    }
                }
                //Si la longitud de la serpiente es mayor a 1

                else
                {
                    //Almacenar la posicion en otra variable (para no perderlo)

                    var posAux2 = new Punto(posAux.X, posAux.Y);

                    //La posicion auxiliar tomara la posicion de la serpiente

                    posAux = new Punto(Serpiente[i].X, Serpiente[i].Y);

                    //Cambiar la posicion de la serpiente a la posicion auxiliar 2

                    Serpiente[i] = new Punto(posAux2.X, posAux2.Y);
                }

                //Situar el cursor en la posicion de la serpiente

                Console.SetCursorPosition(Serpiente[i].X, Serpiente[i].Y);

                //Dibujar el cuerpo de la serpiente

                Console.Write("O");
            }

            //Hacemos que canMove sea igual a true

            canMove = true;

            //Llamar a la función que detecta las colisiones

            DetectarColisiones();
        }

        private static void DetectarTeclas()
        {
            while (Juega)
            {
                //Solo se mueve si canMove = true

                if (canMove)
                {
                    if (
                        dir != Direccion.Abajo && Keyboard.IsKeyDown(Key.Up)
                        || dir != Direccion.Abajo && Keyboard.IsKeyDown(Key.W)
                    )
                    {
                        dir = Direccion.Arriba;
                    }
                    else if (
                        dir != Direccion.Arriba && Keyboard.IsKeyDown(Key.Down)
                        || dir != Direccion.Arriba && Keyboard.IsKeyDown(Key.S)
                    )
                    {
                        dir = Direccion.Abajo;
                    }
                    else if (
                        dir != Direccion.Derecha && Keyboard.IsKeyDown(Key.Left)
                        || dir != Direccion.Derecha && Keyboard.IsKeyDown(Key.A)
                    )
                    {
                        dir = Direccion.Izquierda;
                    }
                    else if (
                        dir != Direccion.Izquierda && Keyboard.IsKeyDown(Key.Right)
                        || dir != Direccion.Izquierda && Keyboard.IsKeyDown(Key.D)
                    )
                    {
                        dir = Direccion.Derecha;
                    }
                }
            }
        }

        private static void SpawnComida()
        {
            //Punto de respawn aleatorio para la comida

            Random random = new Random(Guid.NewGuid().GetHashCode());

            //Crear las coordenadas x e y para la comida

            int x,
                y;

            //Iniciar con una posicion aleatoria

            //Definir el rango aleatorio de aparición para la comida

            x = random.Next(0, Console.WindowWidth - 1);
            y = random.Next(0, Console.WindowHeight - 1);

            //Bucle para la comida

            /*Si la posicion en la que se genera la comida ya existe dentro del cuerpo de la
                serpiente se volverá a generar la posicion del respawn de la comida*/

            while (Serpiente.Where(n => n.X == x && n.Y == y).Any())
            {
                //Definir el rango aleatorio de aparición para la comida

                x = random.Next(0, Console.WindowWidth--);
                y = random.Next(0, Console.WindowHeight--);
            }

            //Posicionar comida

            posicionComida = new Punto(x, y);

            //Posicionar cursor para dibujar la comida

            Console.SetCursorPosition(x, y);

            //Dibujar comida

            Console.Write("+");
        }

        private static void DetectarColisiones()
        {
            //Reconocer la cabeza de la serpiente

            var cabezaSerpiente = Serpiente.First();

            //Detectar si la cabeza de la serpiente toco la comida

            if (cabezaSerpiente.X == posicionComida.X && cabezaSerpiente.Y == posicionComida.Y)
            {
                //Sumar puntuación de 10 en 10

                Score += 10;

                //Actualizar Score

                MostrarScore();

                //Comprobar si la longitud de la serpiente es igual a 1

                if (Serpiente.Count == 1)
                {
                    if (dir == Direccion.Arriba)
                    {
                        Serpiente.Add(new Punto(cabezaSerpiente.X, cabezaSerpiente.Y + 1));
                    }
                    else if (dir == Direccion.Abajo)
                    {
                        Serpiente.Add(new Punto(cabezaSerpiente.X, cabezaSerpiente.Y - 1));
                    }
                    else if (dir == Direccion.Izquierda)
                    {
                        Serpiente.Add(new Punto(cabezaSerpiente.X + 1, cabezaSerpiente.Y));
                    }
                    else if (dir == Direccion.Derecha)
                    {
                        Serpiente.Add(new Punto(cabezaSerpiente.X - 1, cabezaSerpiente.Y));
                    }
                }
                //Si la longitud de la serpiente es mayor a 1

                else
                {
                    //Reconocer el ultimo punto del cuerpo de la serpiente

                    var ultimo = Serpiente.Last();

                    //Reconocer el ante ultimo punto del cuerpo del la serpiente

                    var anteUltimo = Serpiente[Serpiente.Count - 2];

                    //Si esta yendo hacia abajo

                    if (ultimo.X == anteUltimo.X && ultimo.Y + 1 == anteUltimo.Y)
                    {
                        Serpiente.Add(new Punto(cabezaSerpiente.X, cabezaSerpiente.Y - 1));
                    }
                    //Si esta yendo hacia arriba

                    else if (ultimo.X - 1 == anteUltimo.X && ultimo.Y - 1 == anteUltimo.Y)
                    {
                        Serpiente.Add(new Punto(cabezaSerpiente.X + 1, cabezaSerpiente.Y + 1));
                    }
                    //Si esta yendo hacia la izquierda

                    else if (ultimo.X == anteUltimo.X && ultimo.Y == anteUltimo.Y)
                    {
                        Serpiente.Add(new Punto(cabezaSerpiente.X, cabezaSerpiente.Y));
                    }
                    //Si esta yendo hacia la derecha

                    else if (ultimo.X + 1 == anteUltimo.X && ultimo.Y == anteUltimo.Y)
                    {
                        Serpiente.Add(new Punto(cabezaSerpiente.X - 1, cabezaSerpiente.Y));
                    }
                }

                //Resetear la posición de la comida

                posicionComida = null;

                //Espaunear comida

                SpawnComida();
            }

            //Comprobar si la cabeza ha colisionado con algo que no sea con sigo misma

            if (
                Serpiente
                    .Where(
                        n =>
                            n.X == Serpiente[0].X
                            && n.Y == Serpiente[0].Y
                            && !n.Equals(Serpiente[0])
                    )
                    .Any()
            )
            {
                Perder();
            }

            //Comprobar si la serpiente se ha chocado con las paredes de la consola

            if (
                Serpiente[0].X <= 0
                || Serpiente[0].X >= Console.WindowWidth - 1
                || Serpiente[0].Y <= 0
                || Serpiente[0].Y >= Console.WindowHeight - 1
            )
            {
                Perder();
            }
        }

        private static void Perder()
        {
            //Poner la condición que hace que el bucle se repita en false para que se termine

            Juega = false;

            //Situar el cursor en la posición 10, 10

            Console.SetCursorPosition(10, 10);

            //Escribir un mensaje de perder

            Console.Write("Game Over!!! | Presiona intro para volver al menú . . .");

            //Esperar a que el jugador pulse intro

            Console.ReadLine();

            //Comprobar si se ha superado el ScoreMaximo

            if (Score > ScoreMaximo)
            {
                //Limpiar consola

                Console.Clear();

                //Pedir al jugador que ingrese su nombre para guardarlo

                Console.WriteLine(
                    "Nueva máxima puntuación: " + Score + ". Ingrese su nombre para guardarlo: "
                );

                //Leer el nombre escrito

                Nombre = Console.ReadLine();

                //Guardar máxima puntuación

                ScoreMaximo = Score;

                //Guardar puntuación máxima

                GuardarScoreMaximo();
            }
        }

        private static void Menu()
        {
            /*char almacena un carácter unicode para iniciarla vacía
                hay poner obligatoriamente (con comillas simples): '\0'*/

            char tecla = '\0';

            while (tecla != 's' && tecla != 'S')
            {
                //Limpiar la consola

                Console.Clear();

                //Mensaje de bienvenida

                Console.WriteLine(
                    "Bienvenido a Snake | Hecho por Raul Catalinas Esteban en C#\n\n\n"
                );

                //Teclas para el menú

                Console.WriteLine("(J)ugar");
                Console.WriteLine("(P)untaje");
                Console.WriteLine("(S)alir\n");

                //Esperar a que el jugador presione una tecla y asignárselo a la variable tecla

                tecla = Console.ReadKey().KeyChar;

                //Comprobar que tecla presiona el jugador

                if (tecla == 'j' || tecla == 'J')
                {
                    Jugar();
                }
                else if (tecla == 'p' || tecla == 'P')
                {
                    Puntage();
                }
            }
        }

        private static void Jugar()
        {
            //Limpiar consola

            Console.Clear();

            //Crear comida

            SpawnComida();

            //Poner la condición para que se repita el bucle principal del juego en true

            Juega = true;

            //Dirección inicial de la serpiente

            dir = Direccion.Izquierda;

            //Reiniciar puntuación

            Score = 0;

            Serpiente = new List<Punto>()
            {
                //La serpiente empieza en el centro

                new Punto(Console.WindowWidth / 2, Console.WindowHeight / 2)
            };

            //Correr en paralelo la detección de teclas

            Thread threadTeclas = new Thread(DetectarTeclas);
            threadTeclas.SetApartmentState(ApartmentState.STA);
            threadTeclas.Start();

            //Mostrar puntuación

            MostrarScore();

            //Bucle principal del juego

            while (Juega)
            {
                Mover();
                Thread.Sleep(100);
            }
        }

        private static void MostrarScore()
        {
            //Situar cursor arriba a la izquierda

            Console.SetCursorPosition(0, 0);

            //Mostrar Score en pantalla

            Console.WriteLine($"Score: {Score}");
        }

        private static void LeerScoreMaximo()
        {
            //Comprobar si el archivo existe

            if (File.Exists("Score.txt"))
            {
                //Leer archivo

                string[] text = File.ReadAllLines("Score.txt");

                //Comprobar si hay 2 o mas lineas

                if (text.Length >= 2)
                {
                    //Asignar puntuación a la variable que guarda la puntuación máxima

                    ScoreMaximo = Convert.ToInt32(text[0]);

                    /*Asignar el nombre que ha introducido el usuario a la variable que guarda
                        el nombre de quien ha conseguido la puntuación máxima*/

                    Nombre = text[1];
                }
            }
        }

        private static void Puntage()
        {
            //Limpiar consola

            Console.Clear();

            //Comprobar si hay alguna puntuación máxima

            if (Nombre != null)
            {
                //Mostrar puntaje máximo

                Console.WriteLine($"Score mas alto: {ScoreMaximo} Hecho por:  {Nombre}");
            }
            //En caso de que no haya ninguna puntuación máxima

            else
            {
                //Escribir mensaje

                Console.Write("No hay ninguna puntuación máxima almacenada");
            }

            //Dejar espacio en banco

            Console.WriteLine("\n");

            //Escribir mensaje para que el usuario sepa como volver al menú

            Console.WriteLine("Pulsa alguna tecla para volver al menú . . .");

            //Esperar a que se presione alguna tecla

            Console.ReadKey();
        }

        private static void GuardarScoreMaximo()
        {
            //Variable para guardar la máxima puntuación

            string texto = ScoreMaximo + "\n" + Nombre;

            //Crear archivo para guardar la puntuación máxima

            File.WriteAllText("Score.txt", texto);
        }
    }
}
