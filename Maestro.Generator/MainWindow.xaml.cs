using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Maestro.Common.Model;
using Maestro.Generator.Annotations;

namespace Maestro.Generator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string FileFilter = "Maestro Compressed Database|*.mtrcp|Maestro Database|*.mtrdb";
        private const string LastFileConfig = "LastFile";
        private readonly Configuration configuration;
        private readonly Stack<object> undoes = new Stack<object>();
        private readonly Stack<object> redoes = new Stack<object>();
        private bool disableUndo = false;
        private bool _isDirty = false;


        public DataContext Context { get; set; } = new DataContext();

        public MainWindow(string filename = null)
        {
            configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(filename) && File.Exists(filename))
            {
                Context.Load(filename);
                UpdateSettings();
            }
            else if (configuration.AppSettings.Settings.AllKeys.Contains(LastFileConfig) && File.Exists(configuration.AppSettings.Settings[LastFileConfig].Value))
                Context.Load(ConfigurationManager.AppSettings.Get(LastFileConfig));


            ModelBase.OnChanging += ModelBase_OnChanging;
            DataContext = Context;
            OnPropertyChanged(nameof(WindowTitle));
        }

        private void ModelBase_OnChanging(object delta)
        {
            if (!disableUndo)
            {
                undoes.Push(delta);
                redoes.Clear();
            }

            switch (delta)
            {
                case ModelBase.PropertyChange change:
                    LogsTxt.AppendText($"{change.Origin} changed value of {change.PropertyName} ({change.Type.Name}) from {change.PreviousValue} to {change.NewValue}{Environment.NewLine}");
                    break;
                case ModelBase.ListAddition addition:
                    LogsTxt.AppendText($"{addition.Added} added to {addition.ListName()}{Environment.NewLine}");
                    break;
                case ModelBase.ListSuppression suppression:
                    LogsTxt.AppendText($"{suppression.Suppressed} removed from {suppression.ListName()}{Environment.NewLine}");
                    break;
                default:
                    throw new Exception();
            }
            LogsTxt.ScrollToEnd();
            IsDirty = true;
        }

        private void ResetLogs()
        {
            LogsTxt.Clear();
            undoes.Clear();
            redoes.Clear();
        }

        private void New(object sender, RoutedEventArgs e)
        {
            if (IsDirty)
            {
                switch (MessageBox.Show("Save before creating new ?", "File is not saved",
                    MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Yes:
                        Save(this, new RoutedEventArgs());
                        break;
                    default:
                        return;
                }
            }

            Context.Reset();
            IsDirty = false;
            ResetLogs();
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            if (IsDirty)
            {
                switch (MessageBox.Show("Save before opening ?", "File is not saved",
                    MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Yes:
                        Save(this, new RoutedEventArgs());
                        break;
                    default:
                        return;
                }
            }

            var openFileDialog = new OpenFileDialog { Filter = FileFilter };
            var result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Context.Load(openFileDialog.FileName);
                IsDirty = false;
                UpdateSettings();
                ResetLogs();
            }
        }
        private void Save(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Context.FileName))
                SaveAs(this, e);
            else
            {
                Context.Save();
                IsDirty = false;
            }
        }
        private void SaveAs(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog { Filter = FileFilter };
            var result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Context.Save(saveFileDialog.FileName);
                IsDirty = false;
                UpdateSettings();
            }
        }
        private void Quit(object sender, RoutedEventArgs e)
        {
            Save(sender, e);
            Environment.Exit(0);
        }

        private void UpdateSettings()
        {
            configuration.AppSettings.Settings.Remove(LastFileConfig);
            configuration.AppSettings.Settings.Add(LastFileConfig, Context.FileName);
            configuration.Save();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (IsDirty)
            {
                switch (MessageBox.Show("Save before closing ?", "File is not saved", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Yes:
                        Save(this, new RoutedEventArgs());
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }

            base.OnClosing(e);
        }

        private void Generate(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Context.FileName))
                Save(sender, e);

            Context.Generate();
        }

        public bool IsDirty
        {
            get => _isDirty;
            private set { _isDirty = value; OnPropertyChanged(nameof(IsDirty)); OnPropertyChanged(nameof(WindowTitle)); }
        }

        private void Undo(object sender, RoutedEventArgs e)
        {
            if (undoes.Count != 0)
            {
                var delta = undoes.Pop();
                disableUndo = true;
                switch (delta)
                {
                    case ModelBase.PropertyChange change:
                        change.Undo();
                        redoes.Push(change);
                        break;
                    case ModelBase.ListAddition addition:
                        disableUndo = true;
                        addition.Undo();
                        redoes.Push(addition.Invert());
                        disableUndo = false;
                        break;
                    case ModelBase.ListSuppression suppression:
                        disableUndo = true;
                        suppression.Undo();
                        redoes.Push(suppression.Invert());
                        disableUndo = false;
                        break;
                    default:
                        throw new Exception();
                }
                disableUndo = false;
            }
            OnPropertyChanged(nameof(WindowTitle));

        }

        private void Redo(object sender, RoutedEventArgs e)
        {
            if (redoes.Count != 0)
            {
                var delta = redoes.Pop();
                switch (delta)
                {
                    case ModelBase.PropertyChange change:
                        change.Redo();
                        break;
                    case ModelBase.ListAddition addition:
                        disableUndo = true;
                        addition.Undo();
                        undoes.Push(addition.Invert());
                        disableUndo = false;
                        break;
                    case ModelBase.ListSuppression suppression:
                        disableUndo = true;
                        suppression.Undo();
                        undoes.Push(suppression.Invert());
                        disableUndo = false;
                        break;
                    default:
                        throw new Exception();
                }
            }
            OnPropertyChanged(nameof(WindowTitle));
        }

        public string WindowTitle
        {
            get
            {
                var dirty = IsDirty ? "*" : string.Empty;
                return $"Maestro Generator : {dirty}{Context.FileName}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
