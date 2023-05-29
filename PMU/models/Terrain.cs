using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace PMU.models
{
    public class Terrain : Panel
    {
        private List<Horses> horses;
        private Timer timer;
        private bool raceStarted = false;
        private int lapsToWin = 3; // Nombre de tours requis pour gagner
        private double enduranceThreshold = 0.7; // Seuil de distance pour activer l'endurance

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

            Button startButton = new Button();
            startButton.Text = "Start";
            startButton.Location = new Point(10, 10);
            startButton.Click += StartButton_Click;
            Controls.Add(startButton);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            raceStarted = !raceStarted; // Inverser l'état de la course (démarrer ou arrêter)

            if (raceStarted)
            {
                timer.Start(); // Démarrer la course
            }
            else
            {
                timer.Stop(); // Arrêter la course
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(ClientRectangle);
            Region = new Region(path);

            int x = 90;
            int y = 90;

            using (Pen startLinePen = new Pen(Color.White, 2))
            {
                int startLineY = ClientRectangle.Y + ClientRectangle.Height / 2;
                int startLineX = ClientRectangle.X; // Début de la ligne de départ (limite du terrain)

                // Calculer la position X de la fin de la ligne de départ (limite de la piste blanche)
                double endLineX = ClientRectangle.X + ClientRectangle.Width - (x * 6.8);

                Point startPoint = new Point(startLineX, startLineY);
                Point endPoint = new Point((int)endLineX, startLineY);

                e.Graphics.DrawLine(startLinePen, startPoint, endPoint);
            }

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

            int startLineY = ClientRectangle.Y + ClientRectangle.Height / 2;
            int startLineX = ClientRectangle.X; // Début de la ligne de départ (limite du terrain)

            // Calculer la position X de la fin de la ligne de départ (limite de la piste blanche)
            double endLineX = ClientRectangle.X + ClientRectangle.Width - (90 * 6.8);

            horses.Add(new Horses("Cheval 1", startLineX, startLineY, 20, 20, Color.Red, 3, 50)); // Ajout de l'endurance pour le premier cheval
            horses.Add(new Horses("Cheval 2", startLineX + 30, startLineY, 20, 20, Color.Blue, 3, 20)); // Ajout de l'endurance pour le deuxième cheval
            horses.Add(new Horses("Cheval 3", startLineX + 70, startLineY, 20, 20, Color.Green, 4, 10)); // Ajout de l'endurance pour le troisième cheval

            // Ajoutez plus de chevaux selon vos besoins

            return horses;
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            if (!raceStarted)
            {
                return; // Ne rien faire si la course n'a pas commencé
            }

            int finishCount = 0; // Compteur de chevaux ayant terminé la course

            foreach (Horses horse in horses)
            {
                if (horse.ToursEffectues >= lapsToWin)
                {
                    finishCount++;

                   
                }

                if (horse.Speed == 0)
                {
                    continue; // Passer au cheval suivant si celui-ci est déjà arrêté
                }

                // Calculer la nouvelle position du cheval en fonction de sa vitesse
                double centerX = ClientRectangle.Width / 2.0;
                double centerY = ClientRectangle.Height / 2.0;
                double angle = Math.Atan2(horse.Y - centerY, horse.X - centerX);


                // Calculer les nouvelles coordonnées du cheval en utilisant une rotation autour du centre de la piste
                double distance = horse.Speed * Math.PI / 180.0; // Utilisation d'une vitesse constante
                double newX = centerX + distance * Math.Cos(angle);
                double newY = centerY + distance * Math.Sin(angle);

                // Vérifier si le cheval a franchi la ligne de départ
                double endLineX = ClientRectangle.X + ClientRectangle.Width - (90 * 6.8);
                if (newX >= endLineX && horse.X < endLineX)
                {
                    horse.ToursEffectues++; // Incrémenter le compteur de tours pour ce cheval

                    if (horse.ToursEffectues >= lapsToWin)
                    {
                        finishCount++; // Incrémenter le compteur de chevaux ayant terminé
                    }
                }

                // Vérifier si les nouvelles coordonnées sont en dehors de la piste
                double distanceFromCenter = Math.Sqrt(Math.Pow(newX - centerX, 2) + Math.Pow(newY - centerY, 2));
                if (distanceFromCenter > centerX)
                {
                    // Réduire la distance jusqu'au bord de la piste
                    double scaleFactor = centerX / distanceFromCenter;
                    newX = centerX + (newX - centerX) * scaleFactor;
                    newY = centerY + (newY - centerY) * scaleFactor;
                }

                // Mettre à jour les coordonnées du cheval
                horse.X = (int)newX;
                horse.Y = (int)newY;

                // Vérifier si le cheval a atteint 70% du terrain
                double progressPercentage = (horse.X - ClientRectangle.X) / (endLineX - ClientRectangle.X);
                if (progressPercentage >= 0.7)
                {
                    // Ajouter l'endurance à la vitesse du cheval
                    horse.Speed = horse.Speed + ((horse.Speed * horse.Endurance) / 100);
                }
            }

            // Vérifier si tous les chevaux ont terminé la course
            if (finishCount == horses.Count)
            {
                raceStarted = false;
                timer.Stop();
                MessageBox.Show("Tous les chevaux ont terminé la course !");
                return;
            }

            // Redessiner le terrain
            Invalidate();
            await Task.Delay(50); // Délai avant la prochaine mise à jour
        }
    }
}
