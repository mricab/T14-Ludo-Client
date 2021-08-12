using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameClient;

namespace LudoClient
{
    public partial class Form3 : Form, IGClient3
    {
        private Random random = new Random();
        // Properties
        private GClient GameClient;
        Form2 menu;
        private static Graphics drawer;
        private static Graphics piecesDrawer;
        private static int blockSize = 35;

        public Form3(GClient GameClient, Form2 menu)
        {
            InitializeComponent();
            this.GameClient = GameClient;
            GameClient.RegisterObserver3(this);
            this.menu = menu;
            Piece1Button.Parent = PiecesPictureBox;
            Piece2Button.Parent = PiecesPictureBox;
            Piece3Button.Parent = PiecesPictureBox;
            Piece4Button.Parent = PiecesPictureBox;
        }

        // Control Events
        private void CloseGameButton_Click(object sender, EventArgs e)
        {
            Invoke(LaunchMenuFormHandler, this);
            Invoke(CloseFormHandler, this);
        }

        private void EndTurnButton_Click(object sender, EventArgs e)
        {
            EndTurnButton.Visible = false;
            this.GameClient.EndTurn();
        }

        private void DiceButton_Click(object sender, EventArgs e)
        {
            DiceButton.Image = DiceImageList.Images[0];
            this.GameClient.ThrowDice();
        }

        // IGCLient3
        void IGClient3.OnGBoard(GBoardEvent e)
        {
            //MessageBox.Show("Tablero Recibido", "Confirmación", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            BoardPictureBox.Invoke(DrawBoardHandler, this);
            //MessageBox.Show("Terminando evento", "Confirmación", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
        }

        void IGClient3.OnGUpdate(GUpdateEvent e)
        {
            Invoke(UpdateHandler, this);
        }

        // Form Control
        private delegate void CloseFormDelegate(Form3 form);
        CloseFormDelegate CloseFormHandler = CloseForm;
        private static void CloseForm(Form3 form)
        {
            form.Hide();
        }

        private delegate void LaunchMenuFormDelegate(Form3 form);
        LaunchMenuFormDelegate LaunchMenuFormHandler = LaunchMenuForm;
        private static void LaunchMenuForm(Form3 form)
        {
            //Form2 menu = new Form2(form.GameClient);
            form.menu.Show();
        }

        private delegate void DrawBoardDelegate(Form3 form);
        DrawBoardDelegate DrawBoardHandler = DrawBoard;
        public static void DrawBoard(Form3 form)
        {
            //MessageBox.Show("Dibujando...", "Confirmación", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            int size = GClient.Match.board.size;
            string[] cells = GClient.Match.board.cells;
            //GameBoardFields[,] GameBoard = new GameBoardFields[size, size];
            form.BoardPictureBox.Image = new Bitmap(size * blockSize, size * blockSize);
            drawer = Graphics.FromImage(form.BoardPictureBox.Image);
            drawer.Clear(Color.Black);

            for (int i = 0; i < size; i++) // y
            {
                for (int j = 0; j < size; j++) // x
                {
                    int pos = (size * i) + j;
                    drawer.DrawImage(form.BlocksImageList.Images[int.Parse(cells[pos])], j * blockSize, i * blockSize);
                }
            }
        }

        private delegate void UpdateDelegate(Form3 form);
        UpdateDelegate UpdateHandler = UpdateBoard;
        public static void UpdateBoard(Form3 form)
        {
            if (GClient.Match.playing)
            {
                form.TurnLabel.Visible = false;
                //form.EndTurnButton.Visible = true;
                form.DiceButton.Image = form.DiceImageList.Images[GClient.Match.dice];
                if (GClient.Match.canThrow) { form.DiceButton.Enabled = true; } else { form.DiceButton.Enabled = false; }                
                DrawPieces(form);
                ShowPiecesButtons(form);
            }
            else
            {
                form.TurnLabel.Text = "Esperando";
                form.TurnLabel.Visible = true;
                //form.EndTurnButton.Visible = false;
                form.DiceButton.Image = form.DiceImageList.Images[GClient.Match.dice];
                form.DiceButton.Enabled = false;
                DrawPieces(form);
                HidePiecesButtons(form);
            }
        }

        private static void DrawPieces(Form3 form)
        {
            int[] pieces = GClient.Match.pieces;
            int size = GClient.Match.board.size;

            form.PiecesPictureBox.Parent = form.BoardPictureBox;
            form.PiecesPictureBox.BackColor = Color.Transparent;
            form.PiecesPictureBox.Visible = true; 
            form.PiecesPictureBox.Image = new Bitmap(size * blockSize, size * blockSize);
            piecesDrawer = Graphics.FromImage(form.PiecesPictureBox.Image);
            piecesDrawer.Clear(Color.Transparent);

            for (int i = 0; i < pieces.Length; i++)
            {
                int piece = pieces[i];
                int y = piece / size; // pos = (y * size) + x
                int x = piece - (y * size);
                piecesDrawer.DrawImage(form.PiecesImageList.Images[i / 4], x * blockSize, y * blockSize);
            }
        }

        private static void ShowPiecesButtons(Form3 form)
        {
            int x; int y;
            int size = GClient.Match.board.size;       
            int[] myPieces = new int[4]; Array.Copy(GClient.Match.pieces, GClient.Match.playerNum*4, myPieces, 0, 4);

            int piece1 = myPieces[0]; y = piece1 / size;  x = piece1 - (y * size); 
            form.Piece1Button.Location = new Point(x * blockSize, y * blockSize); form.Piece1Button.Visible = GClient.Match.canMove[0];

            int piece2 = myPieces[1]; y = piece2 / size; x = piece2 - (y * size);
            form.Piece2Button.Location = new Point(x * blockSize, y * blockSize); form.Piece2Button.Visible = GClient.Match.canMove[1];

            int piece3 = myPieces[2]; y = piece3 / size; x = piece3 - (y * size);
            form.Piece3Button.Location = new Point(x * blockSize, y * blockSize); form.Piece3Button.Visible = GClient.Match.canMove[2];

            int piece4 = myPieces[3]; y = piece4 / size; x = piece4 - (y * size);
            form.Piece4Button.Location = new Point(x * blockSize, y * blockSize); form.Piece4Button.Visible = GClient.Match.canMove[3];
        }

        private static void HidePiecesButtons(Form3 form)
        {
            form.Piece1Button.Visible = false;
            form.Piece2Button.Visible = false;
            form.Piece3Button.Visible = false;
            form.Piece4Button.Visible = false;
        }

        // Pieces Buttons
        private void Piece1Button_Click(object sender, EventArgs e)
        {
            this.GameClient.MovePiece(0);
            HidePiecesButtons(this);
        }

        private void Piece2Button_Click(object sender, EventArgs e)
        {
            this.GameClient.MovePiece(1);
            HidePiecesButtons(this);
        }

        private void Piece3Button_Click(object sender, EventArgs e)
        {
            this.GameClient.MovePiece(2);
            HidePiecesButtons(this);
        }

        private void Piece4Button_Click(object sender, EventArgs e)
        {
            this.GameClient.MovePiece(3);
            HidePiecesButtons(this);
        }
    }
}
