using System;
using System.IO;
using System.Windows.Forms;

namespace New_app
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Метод для выдачи готового оповещения 
        void Mess(string line1, string line2)
        {
            MessageBox.Show(line1, line2, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Метод удаления файла
        void FileDel(string line)
        {
            // При отсутствии файла выводим сообщение об ошибке и очищаем поля от информации
            if (!File.Exists(line))
            {
                MessageBox.Show("Файл не найден", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TextBox.Text = null;
                label2.Visible = false;
                label2.Text = null;
            }
            else
            {
                // При наличии файла спрашиваем у пользователя о его решении
                DialogResult result = MessageBox.Show("Вы уверены?", null, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    File.Delete(line);
                    if (!File.Exists(line))
                    {
                        // Если пользователь ответил согласием - удаляем файл и выводим оповещение об удалении
                        Mess("Файл успешно удален", "");
                        TextBox.Text = null;
                        label1.Visible = false;
                        label2.Text = null;
                        button.Enabled = false;
                    }
                    else
                    {
                        // Если произошла ошибка и файл по какой-то причине не удалось удалить - выводим оповещение об ошибке
                        Mess("Произошел сбой при попытке удалить файл", "Ошибка!");
                    }
                }
                else
                {
                    //Если пользователь отказался удалять файл
                    Mess("Отмена операции", "Внимание!");
                }
            }
        }

        // Событие загрузки формы
        private void Form1_Load(object sender, EventArgs e)
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(TextBox, "Введите полный путь");
            t.SetToolTip(bunifuThinButton21, "Открыть меню выбор файла");
            label1.Visible = false;
        }

        // Диалоговое окно с выбором файла и запись пути в textbox
        private void BunifuThinButton21_Click(object sender, EventArgs e)
        {
            OpenFileDialog myFile = new OpenFileDialog
            {
                Title = "Выбор файла"
            };
            if (myFile.ShowDialog() == DialogResult.OK)
            {
                TextBox.Text = Convert.ToString(myFile.FileName);
            }
        }

        //Информация о файле при вставке пути на файл вручную
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            // Проверка на наличие чего-либо в textbox
            if (TextBox.Text != null)
            {
                TextBox.Text = TextBox.Text.Trim('"');
                button.Enabled = true;
                // Проверка файла на существование и выдача информации
                if(File.Exists(TextBox.Text))
                {
                    label1.Visible = true;
                    label2.Text = $"Создан: " + File.GetCreationTime(TextBox.Text) +
                                   "\n Открывался: " + File.GetLastAccessTime(TextBox.Text) +
                                   "\n Изменялся: " + File.GetLastWriteTime(TextBox.Text) +
                                   "\n Размер файла: " + (Math.Round(Convert.ToDouble(File.ReadAllBytes(TextBox.Text).Length) / 1048576.0, 2)) + " мбайт";
                }
                else
                {
                    label1.Visible = false;
                    label2.Text = null;
                }
            }
            //Если textbox стал пустым - скрываем лишнюю информацию
            else
            {
                label1.Visible = false;
                label2.Text = null;
                button.Enabled = false;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            FileDel(TextBox.Text);
        }
    }
}
