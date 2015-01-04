using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WindowsFormsAero.TaskDialog;

namespace TongJi.City
{
    public static class DocumentManager
    {
        private static CityDocument _activeDocument = CityDocument.New();
        public static CityDocument ActiveDocument { get { return _activeDocument; } }

        public static bool Switch(CityDocument doc)
        {
            if (ActiveDocument.Modified)
            {
                bool save = false;
                if (Environment.OSVersion.Version.Major >= 6) // running in win7
                {
                    var result = TaskDialog.Show("Save document", "CityGUI", "Current document has been modified. Do you want to save? ", TaskDialogButton.Yes | TaskDialogButton.No | TaskDialogButton.Cancel, TaskDialogIcon.SecurityWarning);
                    if (result == Result.Yes)
                    {
                        save = true;
                    }
                    else if (result == Result.Cancel)
                    {
                        return false;
                    }
                }
                else // running in xp
                {
                    var result = MessageBox.Show("Current document has been modified. Do you want to save? ", "CityGUI", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        save = true;
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return false;
                    }
                }
                if (save)
                {
                    if (ActiveDocument.HasFile)
                    {
                        ActiveDocument.Save();
                    }
                    else
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Title = "Save CityXML";
                        sfd.Filter = "CityXML File (*.cml)|*.cml";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            ActiveDocument.SaveAs(sfd.FileName);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            _activeDocument = doc;
            return true;
        }
    }

    public class CityDocument
    {
        public string FileName { get; private set; }
        public bool HasFile { get; private set; }
        public CityDistrict Content { get; private set; }

        private int _hash;

        public string ShortName
        {
            get
            {
                try
                {
                    return FileName.Substring(FileName.LastIndexOf('\\') + 1);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public bool Modified
        {
            get
            {
                return true;
            }
        }

        private CityDocument()
        {
        }

        private void Snapshot()
        {
            _hash = Content.GetHashCode();
        }

        public static CityDocument New()
        {
            CityDocument doc = new CityDocument();
            doc.FileName = string.Empty;
            doc.HasFile = false;
            doc.Content = new CityDistrict();
            doc.Snapshot();
            return doc;
        }

        public static CityDocument Open(string fileName)
        {
            CityDocument doc = new CityDocument();
            doc.FileName = fileName;
            doc.HasFile = true;
            doc.Content = CityDistrict.LoadCml(fileName);
            doc.Snapshot();
            return doc;
        }

        public void Save()
        {
            Content.SaveCml(FileName);
            Snapshot();
        }

        public void SaveAs(string fileName)
        {
            Content.SaveCml(fileName);
            Snapshot();
        }
    }
}
