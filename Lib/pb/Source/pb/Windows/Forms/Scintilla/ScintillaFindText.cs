using System.Collections.Generic;
using ScintillaNET;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace pb.Windows.Forms
{
    public class ScintillaFindPosition
    {
        public int Start;
        public int End;
    }

    public partial class ScintillaFindText
    {
        private Scintilla _scintillaControl = null;
        private int _scintillaIndicator = 8; // Indicators 0-7 could be in use by a lexer. so we'll use indicator 8 to highlight words.
        private string _text = null;
        private SearchFlags _searchFlags = SearchFlags.None;
        private bool _search = false;
        //private int _initialPosition = 0;
        private List<ScintillaFindPosition> _positions = new List<ScintillaFindPosition>();
        private int _firstPositionIndex = -1;
        private int _positionIndex = -1;
        private int _currentPosition = -1;
        private bool _notification = true;
        //public Action<string, SearchFlags> FindAction = null;
        //public Action FindNextAction = null;
        private ScintillaFindForm _findForm = null;
        private Form _parentForm = null;

        public Action<string> StatusChange = null;

        //int scintillaIndicator = 0
        public ScintillaFindText(Scintilla scintillaControl)
        {
            _scintillaControl = scintillaControl;
            InitScintillaControl();
            //InitForm();
            InitFindForm();
            InitScintillaControlEvent();
            ControlFindForm.Find(_scintillaControl, InitParentForm);
        }

        private void InitScintillaControl()
        {
            // Update indicator appearance
            _scintillaControl.Indicators[_scintillaIndicator].Style = IndicatorStyle.StraightBox;
            _scintillaControl.Indicators[_scintillaIndicator].Under = true;
            _scintillaControl.Indicators[_scintillaIndicator].ForeColor = Color.Green;
            _scintillaControl.Indicators[_scintillaIndicator].OutlineAlpha = 50;
            _scintillaControl.Indicators[_scintillaIndicator].Alpha = 30;
        }

        private void InitFindForm()
        {
            _findForm = new ScintillaFindForm();
            _findForm.SetFindParam = SetFindParam;
            _findForm.FindNext = FindNext;
        }

        private void InitParentForm(Form form)
        {
            _parentForm = form;
        }

        public void OpenFindForm()
        {
            string word = _scintillaControl.zGetCurrentWord();
            if (word != null)
                _findForm.SetText(word);
            _findForm.Show(_parentForm);
        }

        public void HideFindForm()
        {
            _findForm.Hide();
        }

        private void InitScintillaControlEvent()
        {
            // set _search = false if text is modified
            // scintillaControl_BeforeDelete scintillaControl_BeforeInsert
            _scintillaControl.BeforeDelete += (sender, eventArgs) => _search = false;
            _scintillaControl.BeforeInsert += (sender, eventArgs) => _search = false;
        }

        public void SetFindParam(string text, SearchFlags searchFlags = SearchFlags.None)
        {
            if (text != _text || searchFlags != _searchFlags)
            {
                _text = text;
                _searchFlags = searchFlags;
                _search = false;
            }
            //FindNext();
            //else if (_positionIndex != -1)
            //{
            //    _positionIndex++;
            //    if (_positionIndex == _positions.Count)
            //        _positionIndex = 0;
            //}
            //if (_positionIndex != -1)
            //{
            //    FindPosition findPosition = _positions[_positionIndex];
            //    _scintillaControl.GotoPosition(findPosition.Start);
            //    _scintillaControl.SelectionStart = findPosition.Start;
            //    _scintillaControl.SelectionEnd = findPosition.End;
            //}
        }

        public bool FindPrevious()
        {
            return Find(false);
        }

        public void FindNext()
        {
            Find(true);
        }

        public bool Find(bool next)
        {
            bool found = false;
            if (!_search)
            {
                // Remove all uses of our indicator
                _scintillaControl.IndicatorCurrent = _scintillaIndicator;
                _scintillaControl.IndicatorClearRange(0, _scintillaControl.TextLength);
                _scintillaControl.SearchFlags = _searchFlags;
                //int position = _scintillaControl.CurrentPosition;

                // find all occurence
                _scintillaControl.TargetStart = 0;
                _scintillaControl.TargetEnd = _scintillaControl.TextLength;
                _positions.Clear();
                if (_text != null && _text != "")
                {
                    while (_scintillaControl.SearchInTarget(_text) != -1)
                    {
                        // Mark the search results with the current indicator
                        _scintillaControl.IndicatorFillRange(_scintillaControl.TargetStart, _scintillaControl.TargetEnd - _scintillaControl.TargetStart);
                        _positions.Add(new ScintillaFindPosition { Start = _scintillaControl.TargetStart, End = _scintillaControl.TargetEnd });

                        //if (_positionIndex == -1 && _scintillaControl.TargetStart >= position)
                        //    _positionIndex = _positions.Count - 1;

                        // Search the remainder of the document
                        _scintillaControl.TargetStart = _scintillaControl.TargetEnd;
                        _scintillaControl.TargetEnd = _scintillaControl.TextLength;
                    }
                }
                //if (_positionIndex == -1 && _positions.Count > 0)
                //    _positionIndex = 0;
                _positionIndex = -1;
                _search = true;
            }

            // if cursor has moved search again
            if (_currentPosition != _scintillaControl.CurrentPosition)
                _positionIndex = -1;

            if (_positionIndex == -1 && _positions.Count > 0)
            {
                int position = _scintillaControl.CurrentPosition;
                if (next)
                {
                    int i = 0;
                    foreach (ScintillaFindPosition findPosition in _positions)
                    {
                        if (findPosition.Start >= position)
                        {
                            _positionIndex = i;
                            break;
                        }
                        i++;
                    }
                    if (_positionIndex == -1)
                        _positionIndex = 0;
                }
                else
                {
                    for (int i = _positions.Count -1; i >= 0; i--)
                    {
                        if (_positions[i].Start <= position)
                        {
                            _positionIndex = i;
                            break;
                        }
                    }
                    if (_positionIndex == -1)
                        _positionIndex = _positions.Count - 1;
                }
                _firstPositionIndex = _positionIndex;
                found = true;
            }
            else if (_positionIndex != -1 && _positions.Count > 1)
            {
                int positionIndex = _positionIndex;
                if (next)
                {
                    positionIndex++;
                    if (positionIndex == _positions.Count)
                        positionIndex = 0;
                }
                else
                {
                    positionIndex--;
                    if (positionIndex == -1)
                        positionIndex = _positions.Count - 1;
                }

                if (_firstPositionIndex == -1)
                {
                    _positionIndex = positionIndex;
                    _firstPositionIndex = positionIndex;
                    found = true;
                }
                else if (positionIndex != _firstPositionIndex)
                {
                    _positionIndex = positionIndex;
                    found = true;
                }
                else
                    _firstPositionIndex = -1;
            }
            //if (_positionIndex != -1 && found)
            if (found)
            {
                ScintillaFindPosition findPosition = _positions[_positionIndex];
                _scintillaControl.GotoPosition(findPosition.Start);
                _scintillaControl.SelectionStart = findPosition.Start;
                _scintillaControl.SelectionEnd = findPosition.End;
                _currentPosition = _scintillaControl.CurrentPosition;
                Status("found");
            }
            else
            {
                //string message;
                //if (_positions.Count == 0)
                //    message = string.Format("text was not found :\r\n\"{0}\"", _text);
                //else
                //    message = string.Format("find all occurence of text :\r\n\"{0}\"", _text);
                string message = _positions.Count == 0 ? "text was not found" : "find all occurence of text";
                Status(message);
                Notification(message);
            }
            return found;
        }

        private void Status(string message)
        {
            if (StatusChange == null)
                return;
            message += " : " + GetFindDescription();
            StatusChange(message);
        }

        private void Notification(string message)
        {
            if (!_notification)
                return;
            message += " : \r\n" + string.Format("\"{0}\"", _text);
            MessageBox.Show(message, "find text", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GetFindDescription()
        {
            string description = string.Format("\"{0}\"", _text);
            if (_positions.Count > 0)
                description += string.Format(" ({0}/{1})", _positionIndex + 1, _positions.Count);
            return description;
        }

        //public void ScintillaTextModified()
        //{
        //    _search = false;
        //}

        //private void scintillaControl_BeforeDelete(object sender, BeforeModificationEventArgs e)
        //{
        //    ScintillaTextModified();
        //}

        //private void scintillaControl_BeforeInsert(object sender, BeforeModificationEventArgs e)
        //{
        //    ScintillaTextModified();
        //}
    }
}
