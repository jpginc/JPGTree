﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gdk;
using Gtk;

namespace ConsoleApp1
{
    internal class SearchableTreeView : Grid
    {
        private readonly Label _label;
        private readonly NoTabSearchEntry _search;
        private readonly JpgTreeView _treeview;
        public new int Height = 5;
        public new int Width = 1;

        public SearchableTreeView(Func<bool> AcceptCallback) : this()
        {
            _treeview.AcceptCallback = AcceptCallback;
        }
        public SearchableTreeView()
        {
            _label = new Label("Search: ");
            _search = new NoTabSearchEntry(this)
            {
                Expand = false,
                PlaceholderText = "<ctrl+i> to focus"
            };
            _search.Changed += OnSearchChange;
            _search.Activated += OnSearchSubmit;

            BottomElement = new ScrolledWindow
            {
                ShadowType = ShadowType.EtchedIn,
                Expand = true
            };
            _treeview = new JpgTreeView(_search);
            BottomElement.Add(_treeview);
            Add(_label);
            AttachNextTo(_search, _label, PositionType.Bottom, Width, 1);
            AttachNextTo(BottomElement, _search, PositionType.Bottom, Width, Height);
        }

        private void OnSearchSubmit(object sender, EventArgs e)
        {
            _treeview.HandleSearchReturnKey();
        }

        private void OnSearchChange(object sender, EventArgs e)
        {
            _treeview.UpdateOrder(((SearchEntry) sender).Text);
        }

        public SearchableTreeView SetLabelText(string text)
        {
            _label.Text = text;
            return this;
        }

        public ScrolledWindow BottomElement { get; }

        public void SetChoices(IEnumerable<ITreeViewChoice> treeViewChoices)
        {
            _treeview.SetChoices(treeViewChoices);
        }

        public SearchableTreeView SetMultiSelect(bool b)
        {
            _treeview.SetMultiSelect(b);
            return this;
        }

        public IEnumerable<ITreeViewChoice> GetSelectedItems()
        {
            return _treeview.GetSelectedItems();
        }

        public string GetSearchText()
        {
            return _search.Text;
        }

        public SearchableTreeView Reset()
        {
            _search.Text = "";
            _treeview.SetMultiSelect(false);
            SetChoices(Enumerable.Empty<ITreeViewChoice>());
            return this;
        }

        public void FocusInput()
        {
            _search.GrabFocus();
        }

        public void HandleRotateKeypress(EventKey evnt)
        {
            RoatateAndUpdateChoices(evnt.State == ModifierType.ShiftMask);
        }

        private void RoatateAndUpdateChoices(bool forwardDirection)
        {
            _treeview.RotateItems(forwardDirection);
        }

        public void HandleDone()
        {
            _treeview.HandleDone();
        }

        public void SelectAll()
        {
            _treeview.SelectEverything();
        }

        public void HandlePageDownUp()
        {
            _treeview.GrabFocus();
        }

        public void SetSearchText(string searchText)
        {
            _search.Text = searchText;
            _search.SelectRegion(-1, -1); //set the cursor at the end of the search
        }
    }
}