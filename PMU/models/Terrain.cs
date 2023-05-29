using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PMU.models
{
    public class Terrain : Panel
    {
        private List<Horses> horses;
        private Timer timer;

        public Terrain()
        {
            this.Size = new Size(700, 550);
            this.BackColor = Color.Brown;
            this.Location = new Point(100, 50);

            horses = GetHorses();

            timer = new Timer();
            timer.Interval = 100; // Intervalle de mise à jour en millisecondes
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(ClientRectangle);
            Region = new Region(path);

            int x = 90;
            int y = 90;

            for (int i = 0; i < 1; i++)
            {
                Rectangle ellipseRect = new Rectangle(ClientRectangle.X + x, ClientRectangle.Y + y, ClientRectangle.Width - (x * 2), ClientRectangle.Height - (y * 2));
                using (Pen pen = new Pen(Color.White, 2))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawEllipse(pen, ellipseRect);
                }

                foreach (Horses horse in horses)
                {
                    Rectangle rect = new Rectangle(horse.X, horse.Y, horse.Width, horse.Height);
                    e.Graphics.FillEllipse(new SolidBrush(horse.Color), rect);
                }

                x = x + 70;
                y = y + 70;
            }
        }

        private List<Horses> GetHorses()
        {
            List<Horses> horses = new List<Horses>();

            // Exemple de création de chevaux
            horses.Add(new Horses("Cheval 1", 150, 100, 20, 20, Color.Red, 10, 100));
            horses.Add(new Horses("Cheval 2", 100, 100, 20, 20, Color.Blue, 7, 80));
            horses.Add(new Horses("Cheval 3", 90, 150, 20, 20, Color.Green, 6, 90));

            // Ajoutez plus de chevaux selon vos besoins
            return horses;
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            foreach (Horses horse in horses)
            {
                // Calculer la nouvelle position du cheval en fonction de sa vitesse
                double angle = horse.Speed * Math.PI / 180.0; // Convertir la vitesse en angle en radians
                double centerX = ClientRectangle.Width / 2.0;
                double centerY = ClientRectangle.Height / 2.0;

                // Calculer les nouvelles coordonnées du cheval en utilisant une rotation autour du centre de la piste
                double newX = centerX + (horse.X - centerX) * Math.Cos(angle) - (horse.Y - centerY) * Math.Sin(angle);
                double newY = centerY + (horse.X - centerX) * Math.Sin(angle) + (horse.Y - centerY) * Math.Cos(angle);

                // Vérifier si les nouvelles coordonnées sont en dehors de la piste
                double distanceFromCenter = Math.Sqrt(Math.Pow(newX - centerX, 2) + Math.Pow(newY - centerY, 2));
                if (distanceFromCenter > centerX)
                {
                    // Réduire la distance jusqu'au bord de la piste
                    double scaleFactor = centerX / distanceFromCenter;
                    newX = centerX + (newX - centerX) * scaleFactor;
                    newY = centerY + (newY - centerY) * scaleFactor;
                }

                horse.X = (int)newX;
                horse.Y = (int)newY;
            }

            // Redessiner le terrain
            Invalidate();
            await Task.Delay(50); // Délai avant la prochaine mise à jour
        }


    }
}
