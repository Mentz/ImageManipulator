﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageManipulator {

    public partial class Form1: Form {
        public string originalFile;
        public Bitmap originalImage;
        public Bitmap modifiableImage;
        public Bitmap secondImageForTransition;
        bool isZoom = false;

        public Form1() {
            InitializeComponent();
        }

        public void updatePicture(Bitmap picture) {
            pictureBox1.Image = picture;
        }

        private void imageLoad_Click(object sender, EventArgs e) {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                originalFile = openFileDialog1.FileName;
                Bitmap importedImage = new Bitmap(originalFile);
                originalImage = importedImage;
                modifiableImage = importedImage;
                updatePicture(modifiableImage);
                optionsPanel.Enabled = true;
            }
        }

        private void imageReset_Click(object sender, EventArgs e) {
            modifiableImage = new Bitmap(originalFile);
            updatePicture(modifiableImage);
        }

        private void imageSave_Click(object sender, EventArgs e) {
            MessageBox.Show("Salve como um arquivo formato PNG (*.png)");
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                modifiableImage.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void imageRotate90_Click(object sender, EventArgs e) {
            int h = modifiableImage.Width;
            int w = modifiableImage.Height;
            Bitmap rotated = new Bitmap(w, h);
            for (int i = 0; i < h; i++) {
                for (int j = 0; j < w; j++) {
                    rotated.SetPixel(j, i, modifiableImage.GetPixel(i, w - 1 - j));
                }
            }
            modifiableImage = rotated;
            updatePicture(modifiableImage);
        }

        private void imageRotate_Click(object sender, EventArgs e) {
            if (rotationTextBox.Text == "") { }
            else {
                double angle = Convert.ToDouble(rotationTextBox.Text);
                modifiableImage = RotateImage(modifiableImage, angle);
                updatePicture(modifiableImage);
            }
        }

        private void imageVertFlip_Click(object sender, EventArgs e) {
            int w = modifiableImage.Width;
            int h = modifiableImage.Height;
            Bitmap fliped = new Bitmap(w, h);
            for (int i = 0; i < h; i++) {
                for (int j = 0; j < w; j++) {
                    fliped.SetPixel(j, i, modifiableImage.GetPixel(j, h - 1 - i));
                }
            }
            modifiableImage = fliped;
            updatePicture(modifiableImage);
        }

        private void imageHorizFlip_Click(object sender, EventArgs e) {
            int w = modifiableImage.Width;
            int h = modifiableImage.Height;
            Bitmap fliped = new Bitmap(w, h);
            for (int i = 0; i < h; i++) {
                for (int j = 0; j < w; j++) {
                    fliped.SetPixel(j, i, modifiableImage.GetPixel(w - 1 - j, i));
                }
            }
            modifiableImage = fliped;
            updatePicture(modifiableImage);
        }

        private void imageGrayscale_Click(object sender, EventArgs e) {
            int sx, sy;
            sx = modifiableImage.Width;
            sy = modifiableImage.Height;
            for (int y = 0; y < sy; y++) {
                for (int x = 0; x < sx; x++) {
                    Color oldColor = modifiableImage.GetPixel(x, y);
                    int grayValue = Convert.ToInt32(oldColor.R * 0.299 + oldColor.G * 0.587 + oldColor.B * 0.114);
                    Color newColor = Color.FromArgb(grayValue, grayValue, grayValue);
                    modifiableImage.SetPixel(x, y, newColor);
                }
            }
            updatePicture(modifiableImage);
        }

        private void imageInvertColors_Click(object sender, EventArgs e) {
            int sx, sy;
            sx = modifiableImage.Width;
            sy = modifiableImage.Height;
            for (int y = 0; y < sy; y++) {
                for (int x = 0; x < sx; x++) {
                    Color oldColor = modifiableImage.GetPixel(x, y);
                    Color newColor = Color.FromArgb(255 - oldColor.R, 255 - oldColor.G, 255 - oldColor.B);
                    modifiableImage.SetPixel(x, y, newColor);
                }
            }
            updatePicture(modifiableImage);
        }

        private void imageThresholding_Click(object sender, EventArgs e) {
            int sx, sy;
            sx = modifiableImage.Width;
            sy = modifiableImage.Height;
            Color newColor;
            Color oldColor;
            for (int y = 0; y < sy; y++) {
                for (int x = 0; x < sx; x++) {
                    oldColor = modifiableImage.GetPixel(x, y);
                    if (Convert.ToInt32(oldColor.R * 0.299 + oldColor.G * 0.587 + oldColor.B * 0.114) > illuminanceTrackBar.Value) {
                        newColor = Color.FromArgb(255, 255, 255);
                    }
                    else {
                        newColor = Color.FromArgb(0, 0, 0);
                    }
                    modifiableImage.SetPixel(x, y, newColor);
                }
            }
            updatePicture(modifiableImage);
        }

        private void imageRGBUpdate_Click(object sender, EventArgs e) {
            int sx, sy, newR, newG, newB;
            sx = modifiableImage.Width;
            sy = modifiableImage.Height;
            double rr, rg, rb, gr, gg, gb, br, bg, bb;
            rr = Convert.ToDouble(matrixRr.Text.Replace('.', ','));
            rg = Convert.ToDouble(matrixRg.Text.Replace('.', ','));
            rb = Convert.ToDouble(matrixRb.Text.Replace('.', ','));
            gr = Convert.ToDouble(matrixGr.Text.Replace('.', ','));
            gg = Convert.ToDouble(matrixGg.Text.Replace('.', ','));
            gb = Convert.ToDouble(matrixGb.Text.Replace('.', ','));
            br = Convert.ToDouble(matrixBr.Text.Replace('.', ','));
            bg = Convert.ToDouble(matrixBg.Text.Replace('.', ','));
            bb = Convert.ToDouble(matrixBb.Text.Replace('.', ','));
            for (int y = 0; y < sy; y++) {
                for (int x = 0; x < sx; x++) {
                    Color oldColor = modifiableImage.GetPixel(x, y);
                    newR = (int)Math.Max(Math.Min((oldColor.R * rr + oldColor.G * rg + oldColor.B * rb), 255), 0);
                    newG = (int)Math.Max(Math.Min((oldColor.R * gr + oldColor.G * gg + oldColor.B * gb), 255), 0);
                    newB = (int)Math.Max(Math.Min((oldColor.R * br + oldColor.G * bg + oldColor.B * bb), 255), 0);
                    Color newColor = Color.FromArgb(newR, newG, newB);
                    modifiableImage.SetPixel(x, y, newColor);
                }
            }
            updatePicture(modifiableImage);
        }

        private void transitionLoadSecond_Click(object sender, EventArgs e) {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                Bitmap importedImage = new Bitmap(openFileDialog1.FileName);
                secondImageForTransition = importedImage;
                transitionApply.Enabled = true;
                transitionSlider.Enabled = true;
            }
        }

        private void transitionApply_Click(object sender, EventArgs e) {
            int height = modifiableImage.Height;
            int width = modifiableImage.Width;
            double secondWeight = transitionSlider.Value / 100d;
            double firstWeight = 1d - secondWeight;
            Bitmap secondImage = new Bitmap(secondImageForTransition, width, height);
            Color firstColor, secondColor, newColor;
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    firstColor = modifiableImage.GetPixel(j, i);
                    secondColor = secondImage.GetPixel(j, i);
                    newColor = Color.FromArgb((int)(firstColor.R * firstWeight + secondColor.R * secondWeight),
                                              (int)(firstColor.G * firstWeight + secondColor.G * secondWeight),
                                              (int)(firstColor.B * firstWeight + secondColor.B * secondWeight));
                    modifiableImage.SetPixel(j, i, newColor);
                }
            }
            updatePicture(modifiableImage);
        }

        private void imageZoomToogle_Click(object sender, EventArgs e) {
            if (!isZoom) {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                imageZoomToogle.Text = "Reduzir";
                isZoom = true;
            }
            else {
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                imageZoomToogle.Text = "Ampliar";
                isZoom = false;
            }
        }

        private void imageShowMatrix_Click(object sender, EventArgs e) {
            if (matrixViewer.Visible == false) {
                if (modifiableImage.Height > 100 || modifiableImage.Width > 100) {
                    MessageBox.Show("Imagem muito grande, selecione outra com tamanho menor de 100x100px");
                }
                else {
                    int maxY = modifiableImage.Height;
                    int maxX = modifiableImage.Width;
                    string[,] matriz = new string[maxY, maxX];
                    for (int i = 0; i < maxY; i++) {
                        for (int j = 0; j < maxX; j++) {
                            Color cor = modifiableImage.GetPixel(j, i);
                            matriz[i, j] = "#" + cor.R.ToString("X2") + cor.G.ToString("X2") + cor.B.ToString("X2");
                        }
                    }
                    var rowCount = maxY;
                    var rowLength = maxX;
                    matrixViewer.ColumnCount = maxX;
                    for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex) {
                        var row = new DataGridViewRow();
                        for (int columnIndex = 0; columnIndex < rowLength; ++columnIndex) {
                            row.Cells.Add(new DataGridViewTextBoxCell() {
                                Value = matriz[rowIndex, columnIndex]
                            });
                        }

                        matrixViewer.Rows.Add(row);
                    }
                    matrixViewer.Visible = true;
                    matrixViewer.Enabled = true;
                    imageShowMatrix.Text = "Esconder matriz";
                }
            }
            else {
                matrixViewer.ColumnCount = 0;
                matrixViewer.RowCount = 0;
                matrixViewer.Visible = false;
                matrixViewer.Enabled = false;
                imageShowMatrix.Text = "Exibir matriz";
            }
        }




        //Funções necessárias

        public static Bitmap RotateImage(Image image, double angle) {
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            Graphics g = Graphics.FromImage(rotatedBmp);
            g.TranslateTransform((float)image.Width / 2, (float)image.Height / 2);
            g.RotateTransform((float)angle);
            g.TranslateTransform(-(float)image.Width / 2, -(float)image.Height / 2);
            g.DrawImage(image, new PointF(0, 0));

            return rotatedBmp;
        }

        private void optionsPanel_Paint(object sender, PaintEventArgs e) {

        }

        private void matrixInput_KeyPress(object sender, KeyPressEventArgs e) {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ',')) {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1)) {
                e.Handled = true;
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1)) {
                e.Handled = true;
            }
        }
    }
}