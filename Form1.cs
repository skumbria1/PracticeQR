using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using ZXing;
using ZXing.QrCode;

namespace PracticeQR
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
            Table.RowHeadersVisible = false;
            Hashtable hints = new Hashtable();
            hints.Add(EncodeHintType.CHARACTER_SET, "utf-8");
        }

        int ID = 1;
        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char key = e.KeyChar;
            if ((key < 65 || key > 90) && (key < 97 || key > 122) && (key < 1040 || key > 1103) && key != 1 && key != 3 && key != 8 && key != 22 && key != 32)
            { e.Handled = true; }
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char key = e.KeyChar;
            if ((key < 65 || key > 90) && (key < 97 || key > 122) && (key < 1040 || key > 1103) && key != 1 && key != 3 && key != 8 && key != 22 && key != 32)
            { e.Handled = true; }
        }

        private void TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            char key = e.KeyChar;
            if ((key < 65 || key > 90) && (key < 97 || key > 122) && (key < 1040 || key > 1103) && (key < 44 || key > 46) && key != 1 && key != 3 && key != 8 && key != 22 && key != 32 && !Char.IsDigit(key))
            { e.Handled = true; }
        }

        private void TextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            char key = e.KeyChar;
            if ((key < 65 || key > 90) && (key < 97 || key > 122) && (key < 1040 || key > 1103) && (key < 44 || key > 46) && key != 1 && key != 3 && key != 8 && key != 22 && key != 32 && !Char.IsDigit(key))
            { e.Handled = true; }
        }

        private void TextBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            char key = e.KeyChar;
            if ((key < 65 || key > 90) && (key < 97 || key > 122) && (key < 1040 || key > 1103) && (key < 44 || key > 46) && key != 1 && key != 3 && key != 8 && key != 22 && key != 32 && !Char.IsDigit(key))
            { e.Handled = true; }
        }

        private void ShowHide(string tag, bool a)
        {
            foreach (Control obj in this.Controls)
            {
                if (obj.Tag == tag)
                {
                    obj.Visible = a;
                    if (obj.GetType() == typeof(TextBox)) { ((TextBox)obj).Text = ""; }
                }
                if (obj.Tag == "Main")
                {
                    obj.Visible = !a;
                }
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddConfirmButton.Text = "Добавить";
            ShowHide("Add", true);
        }

        private void AddBackButton_Click(object sender, EventArgs e)
        {
            ShowHide("Add", false);
            ShowHide("Edit", false);
        }

        private void AddConfirmButton_Click(object sender, EventArgs e)
        {
            bool check = true;
            foreach (Control obj in this.Controls)
            {
                if (obj.Tag == "Add" && obj.GetType() == typeof(TextBox))
                {
                    if(((TextBox)obj).Text == "") { check = false; }
                }
            }
            if(check==true)
            {
                Table.Rows.Add(ID,FIO1TXT.Text, FIO2TXT.Text, AdresTXT.Text, TemaTXT.Text, ContentTXT.Text,"","Создано","");
                ShowHide("Add", false);
                ID++;
            }
            else { MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void ReadButton_Click(object sender, EventArgs e)
        {
            PictureBox.Image = null;
            ReadAddButton.Text = "Добавить";
            ShowHide("Read", true);
        }

        private void ReadBackButton_Click(object sender, EventArgs e)
        {
            ShowHide("Read", false);
        }

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "JPG or PNG|*.jpg;*.png";
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                FileBox1.Text = openfile.SafeFileName;
                FileBox2.Text = openfile.FileName;
                PictureBox.Image = Image.FromFile(openfile.FileName);
            }
        }

        private void ReadAddButton_Click(object sender, EventArgs e)
        {
            bool check = true;
            foreach (Control obj in this.Controls)
            {
                if (obj.Tag == "Read" && obj.GetType() == typeof(TextBox))
                {
                    if (((TextBox)obj).Text == "") { check = false; }
                }
            }
            if (check == true)
            {
                BarcodeReader reader = new BarcodeReader();
                Result result = reader.Decode(new Bitmap(PictureBox.Image)); 

                if (result != null)
                {
                    string decoded = result.ToString();
                    string[] data = { "", "", "", "", "", "", "", "", ""};
                    char[] array = new char[decoded.Length];
                    array = decoded.ToCharArray();
                    int j = 0;
                    for (int i = 0; i < decoded.Length; i++)
                    {
                        if (array[i] == '$') j++;
                    }
                    if (j != 9)
                    {
                        MessageBox.Show("В QR-коде не содержатся данные о записи.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        PictureBox.Image = null;
                        FileBox1.Text = "";
                        FileBox2.Text = "";
                    }
                    else
                    {
                        j = 0;
                        for (int i = 0; i < decoded.Length; i++)
                        {
                            if (array[i] == '$') { j++; continue; }
                            data[j] += array[i].ToString();
                        }
                        string status = "";
                        switch (data[7])
                        {
                            case "с":
                                status = "Создано";
                                break;
                            case "р":
                                status = "Рассмотрено";
                                break;
                            case "о":
                                status = "Отклонено";
                                break;
                            default: break;
                        }
                        bool check1 = false;
                        int equal = 0;
                        for (int i = 0; i < Table.RowCount; i++)
                        {
                            if (data[0] == Table[0, i].Value.ToString()) { check1 = true; equal = i; }
                        }
                        if (ReadAddButton.Text == "Добавить")
                        {
                            if (!check1)
                            {
                                Table.Rows.Add(data[0], data[1], data[2], data[3], data[4], data[5], data[6], status, data[8]);
                                MessageBox.Show("Обращение №" + data[0] + " считано.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ShowHide("Read", false);
                            }
                            else
                            {
                                MessageBox.Show("Обращение №" + data[0] + " уже существует. Воспользуйтесь кнопкой Обновить.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                PictureBox.Image = null;
                                FileBox1.Text = "";
                                FileBox2.Text = "";
                            }
                        }
                        else
                        {
                            if (check)
                            {
                                Table.Rows[equal].Cells[6].Value = data[6];
                                Table.Rows[equal].Cells[7].Value = status;
                                Table.Rows[equal].Cells[8].Value = data[8];
                                ShowHide("Read", false);
                            }
                            else
                            {
                                MessageBox.Show("Идентификаторы записей не совпадают", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                PictureBox.Image = null;
                                FileBox1.Text = "";
                                FileBox2.Text = "";
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("QR-код не распознан.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    PictureBox.Image = null;
                    FileBox1.Text = "";
                    FileBox2.Text = "";
                }
            }
            else { MessageBox.Show("Выберите файл", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void Table_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int a = Table.CurrentRow.Index;
            switch (e.ColumnIndex)
            {
                case 9: //Update
                    PictureBox.Image = null;
                    ReadAddButton.Text = "Обновить";
                    ShowHide("Read", true);
                    break;
                case 10: // Print
                    string register = "";
                    string printreg = "";
                    for (int i = 0; i < 9; i++)
                    {
                        if (i == 7)
                        {
                            switch (Table[7, a].Value.ToString())
                            {
                                case "Создано":
                                    register += "с";
                                    break;
                                case "Рассмотрено":
                                    register += "р";
                                    break;
                                case "Отклонено":
                                    register += "о";
                                    break;
                                default: break;
                            }
                        }
                        else
                        {
                            register += Table[i, a].Value.ToString();
                        }
                        register += "$";
                        printreg += Table.Columns[i].HeaderText + ": " + Table[i, a].Value.ToString() + "\n";
                    }

                    QrCodeEncodingOptions options = new QrCodeEncodingOptions
                    {
                        DisableECI = true,
                        CharacterSet = "UTF-8",
                        Width = 2000,
                        Height = 2000,
                    };
                   
                    var qr = new ZXing.BarcodeWriter();
                    qr.Format = ZXing.BarcodeFormat.QR_CODE;
                    qr.Options = options;
                    Bitmap codeqr = new Bitmap(qr.Write(register.Trim()), 1260, 1260);

                    Bitmap res = new Bitmap(1890, 2673);

                    Graphics g = Graphics.FromImage(res);
                    g.Clear(Color.White);
                    g.DrawImage(codeqr, 0, 0);

                    Rectangle rect = new Rectangle(40, 1260, 1770, 1360);
                    Font font = new Font("Arial", 18);
                    g.DrawString(printreg, font, Brushes.Black, rect, System.Drawing.StringFormat.GenericTypographic);


                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "PNG|*.png|JPEG|*.jpg|BMP|*.bmp";
                    if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
                    {
                        res.Save(save.FileName); 
                    }
                    break;

                case 11: //Edit
                    AddConfirmButton.Text = "Сохранить";
                    ShowHide("Add", true);
                    ShowHide("Edit", true);
                    AddConfirmButton.Visible = false;
                    IDTXT.Text = Table[0,a].Value.ToString();
                    FIO1TXT.Text = Table[1, a].Value.ToString();
                    FIO2TXT.Text = Table[2, a].Value.ToString();
                    AdresTXT.Text = Table[3, a].Value.ToString();
                    TemaTXT.Text = Table[4, a].Value.ToString();
                    ContentTXT.Text = Table[5, a].Value.ToString();
                    ResTXT.Text = Table[6, a].Value.ToString();
                    switch (Table[7, a].Value.ToString())
                    {
                        case "Создано":
                            CheckBox0.Checked = true;
                            break;
                        case "Рассмотрено":
                            CheckBox1.Checked = true;
                            break;
                        case "Отклонено":
                            CheckBox2.Checked = true;
                            break;
                        default: break;
                    }

                    NoteTXT.Text = Table[8, a].Value.ToString();
                    break;
                case 12: //Delete
                    DialogResult result = MessageBox.Show("Удалить запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        Table.Rows.Remove(Table.Rows[a]);
                    }
                    break;
                default: break;
            }
        }

        private void EditSaveButton_Click(object sender, EventArgs e)
        {
            int a = Table.CurrentRow.Index;
            Table.Rows[a].Cells[0].Value = IDTXT.Text;
            Table.Rows[a].Cells[1].Value = FIO1TXT.Text;
            Table.Rows[a].Cells[2].Value = FIO2TXT.Text;
            Table.Rows[a].Cells[3].Value = AdresTXT.Text;
            Table.Rows[a].Cells[4].Value = TemaTXT.Text;
            Table.Rows[a].Cells[5].Value = ContentTXT.Text;
            Table.Rows[a].Cells[6].Value = ResTXT.Text;
            if (CheckBox0.Checked == true) { Table.Rows[a].Cells[7].Value = "Создано"; }
            if (CheckBox1.Checked == true) { Table.Rows[a].Cells[7].Value = "Рассмотрено"; }
            if (CheckBox2.Checked == true) { Table.Rows[a].Cells[7].Value = "Отклонено"; }
            Table.Rows[a].Cells[8].Value = NoteTXT.Text;
            ShowHide("Add", false);
            ShowHide("Edit", false);
        }


        private void CheckBox0_Click(object sender, EventArgs e)
        {
            if (CheckBox0.Checked == true)
            {
                CheckBox1.Checked = false;
                CheckBox2.Checked = false;
            }
            else
            {
                CheckBox0.Checked = true;
            }
        }

        private void CheckBox1_Click(object sender, EventArgs e)
        {
            if (CheckBox1.Checked == true)
            {
                CheckBox0.Checked = false;
                CheckBox2.Checked = false;
            }
            else
            {
                CheckBox1.Checked = true;
            }
        }

        private void CheckBox2_Click(object sender, EventArgs e)
        {
            if (CheckBox2.Checked == true)
            {
                CheckBox0.Checked = false;
                CheckBox1.Checked = false;
            }
            else
            {
                CheckBox2.Checked = true;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "TXT|*.txt";
            if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = save.FileName;
                StreamWriter writer = new StreamWriter(path);
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        writer.Write(Table.Rows[i].Cells[j].Value.ToString() + "@");
                    }
                    writer.Write("\n");
                }
                writer.Close();
                MessageBox.Show("Saved to " + path);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "TXT|*.txt";
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Table.Rows.Clear();
                string path = open.FileName;
                StreamReader reader = new StreamReader(path);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split('@');
                    int index = Table.Rows.Add();
                    Table.Rows[index].SetValues(values);
                }
                reader.Close();
            }
        }
    }
}
