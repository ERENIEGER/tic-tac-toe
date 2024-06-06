using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace TicTacToc
{
    public partial class MainWindow : Window
    {
        public GameBoard MyGameBoard = new GameBoard();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = MyGameBoard;
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {//Обновляет пользовательский интерфейс и вызывает обновление GameBoard.
            var clickedButton = sender as Button;

            if (MyGameBoard.currentPlayer == GameBoard.CurrentPlayer.X)
            {
                clickedButton.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#811717"));
            }
            else if (MyGameBoard.currentPlayer == GameBoard.CurrentPlayer.O)
            {
                clickedButton.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#126712"));
            }
            clickedButton.Background = Brushes.WhiteSmoke;
            clickedButton.Content = MyGameBoard.currentPlayer;
            clickedButton.IsHitTestVisible = false;

            MyGameBoard.UpdateBoard(clickedButton.Name);
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {//Перезапускает игру
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(MyGrid) - 1; i++) // This loop iterates through all the buttons/tiles in the grid and sets changed properties to default
            {
                var child = VisualTreeHelper.GetChild(MyGrid, i) as Button;
                child.Content = null;
                child.IsHitTestVisible = true;
                child.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
            }
            MyGameBoard = new GameBoard();
            this.DataContext = MyGameBoard;
        }
    }

    public class GameBoard : INotifyPropertyChanged
    {
        //Переменные, которые будут использоваться
        public enum CurrentPlayer
        {
            X = 1,
            O
        }

        private int turn = 1;
        public CurrentPlayer currentPlayer = CurrentPlayer.X;
        private bool hasWon = false;
        public bool HasWon
        {
            get { return hasWon; }
            set { hasWon = value; NotifyPropertyChanged("HasWon"); }
        }

        private Dictionary<string, int> board = new Dictionary<string, int>()
            {
                {"TopXLeft",0 },
                {"TopXMiddle",0 },
                {"TopXRight",0 },
                {"CenterXLeft",0 },
                {"CenterXMiddle",0 },
                {"CenterXRight",0 },
                {"BottomXLeft",0 },
                {"BottomXMiddle",0 },
                {"BottomXRight",0 }
            };

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private bool CheckIfWon(string buttonName)
        {//Вызывает все методы, проверяющие, выиграна ли игра.
            if (WonInRow(buttonName))
            {
                return true;
            }
            else if (WonInColumn(buttonName))
            {
                return true;
            }
            else if (WonInDiagonal(buttonName))
            {
                return true;
            }
            else
                return false;
        }

        private bool WonInRow(string name)
        {//Проверяет, выиграл ли игрок только что, имея три фигуры в ряду плитки.
            string row = name.Substring(0, name.IndexOf('X') - 1);

            foreach (var element in board)
            {
                string keyName = element.Key;
                string rowOfElement = keyName.Substring(0, keyName.IndexOf('X') - 1);

                if (rowOfElement == row)
                {
                    if (element.Value != (int)currentPlayer)
                        return false;
                }
            }

            return true;
        }

        private bool WonInColumn(string name)
        {//Проверяет, выиграл ли игрок только три фигуры в столбце плитки.
            string column = name.Substring(name.IndexOf('X') + 1);

            foreach (var element in board)
            {
                string keyName = element.Key;
                string columnOfElement = keyName.Substring(keyName.IndexOf('X') + 1);

                if (columnOfElement == column)
                {
                    if (element.Value != (int)currentPlayer)
                        return false;
                }
            }

            return true;
        }

        private bool WonInDiagonal(string name)
        {//Проверяет, выиграл ли игрок только что, имея три фигуры по диагонали.
            if (name == "TopXLeft" || name == "CenterXMiddle" || name == "BottomXRight")
            {
                if (board["CenterXMiddle"] == (int)currentPlayer && board["BottomXRight"] == (int)currentPlayer && board["TopXLeft"] == (int)currentPlayer)
                {
                    return true;
                }
                else
                    return false;
            }
            if (name == "TopXRight" || name == "CenterXMiddle" || name == "BottomXLeft")
            {
                if (board["CenterXMiddle"] == (int)currentPlayer && board["BottomXLeft"] == (int)currentPlayer && board["TopXRight"] == (int)currentPlayer)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        private void UpdateDictionary(string buttonName)
        {//Обновите словарь, изменив значение выбранного тайла (ключа) на значение игрока.
            string tileName = buttonName;

            board[tileName] = (int)currentPlayer;
        }

        public void UpdateBoard(string buttonName)
        {//Управляет логикой игры, обновляя доску и проверяя условия победы, вызываемые при нажатии плитки.
            UpdateDictionary(buttonName);

            if (turn >= 5)//Самый ранний ход, который игрок может выиграть
            {
                if (CheckIfWon(buttonName))
                {
                    HasWon = true;
                }
            }

            turn++;

            if (currentPlayer == CurrentPlayer.X)
                currentPlayer = CurrentPlayer.O;

            else if (currentPlayer == CurrentPlayer.O)
                currentPlayer = CurrentPlayer.X;
        }
    }
}