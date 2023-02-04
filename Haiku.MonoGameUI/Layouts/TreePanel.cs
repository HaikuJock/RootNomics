using Haiku.Collectionspace;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public interface TreeExpanding
    {
        bool IsExpanded { get; set; }
    }

    public interface TreeCellProviding<K, B> where B : TreeExpanding
    {
        Layout CellAtPath(K key, B body);
    }

    public static class TreeTextures
    {
        public static SpriteFrame Plus;
        public static SpriteFrame Minus;
        public static SpriteFrame ButtonActive;
        public static SpriteFrame ButtonNormal;
        public static SpriteFrame ButtonSelected;
    }

    public class TreePanel<K, B> : ScrollPanel where B : TreeExpanding
    {
        public const int ButtonSize = 26;
        public const int ButtonHorizontalMargin = 3;
        public const int ButtonVerticalMargin = 3;
        public const int IndentWidth = ButtonSize + ButtonHorizontalMargin * 2;
        public const int IndentHeight = ButtonSize + ButtonVerticalMargin * 2;
        readonly TreeCellProviding<K, B> cellProvider;
        Tree<K, B> currentTree;

        public TreePanel(
            Rectangle frame,
            int spacing,
            int padding,
            TreeCellProviding<K, B> cellProvider,
            bool includeScrollBar = true, 
            bool showShadows = true) 
            : base(frame, Orientation.Vertical, spacing, padding, includeScrollBar, wrapContent: false, showShadows)
        {
            this.cellProvider = cellProvider;
        }

        public void ReplaceContent(Tree<K, B> tree)
        {
            currentTree = tree;
            var newContent = new List<Layout>();

            RecurseReplaceContent(tree, 0, newContent);

            base.ReplaceContent(newContent);
        }

        void RecurseReplaceContent(Tree<K, B> tree, int indentLevel, List<Layout> newContent)
        {
            foreach (var keyValue in tree)
            {
                var cell = MakeCell(indentLevel, keyValue.Key, keyValue.Value.Body, isLeaf: keyValue.Value.Values.Count == 0);

                newContent.Add(cell);
                if (keyValue.Value.Body.IsExpanded)
                {
                    RecurseReplaceContent(keyValue.Value, indentLevel + 1, newContent);
                }
            }
        }

        Layout MakeCell(int indentLevel, K key, B body, bool isLeaf)
        {
            var layout = new LinearLayout(Orientation.Horizontal);

            if (isLeaf)
            {
                ++indentLevel;
            }

            var spacing =
                indentLevel > 0
                ? IndentWidth * indentLevel
                : 1;
            var spacingLayout = new Layout(spacing, IndentHeight);

            layout.AddChild(spacingLayout);

            if (!isLeaf)
            {
                var buttonContainerFrame = new Rectangle(0, 0, IndentWidth, IndentHeight);
                var buttonContainerLayout = new Layout(buttonContainerFrame);
                var expandNodeButton = new ToggleButton(new Rectangle(0, 0, ButtonSize, ButtonSize))
                {
                    IsOn = body.IsExpanded
                };
                expandNodeButton.SetBackground(TreeTextures.ButtonNormal, TreeTextures.ButtonActive, TreeTextures.ButtonSelected);

                if (body.IsExpanded)
                {
                    expandNodeButton.SetForeground(TreeTextures.Minus);
                }
                else
                {
                    expandNodeButton.SetForeground(TreeTextures.Plus);
                }
                expandNodeButton.Action += (_) => OnNodeToggled(expandNodeButton, body);
                buttonContainerLayout.AddChild(expandNodeButton);
                expandNodeButton.CenterInParent();
                layout.AddChild(buttonContainerLayout);
            }

            layout.AddChild(cellProvider.CellAtPath(key, body));

            return layout;
        }

        void OnNodeToggled(ToggleButton toggle, B body)
        {
            body.IsExpanded = toggle.IsOn;
            ReplaceContent(currentTree);
        }
    }
}
