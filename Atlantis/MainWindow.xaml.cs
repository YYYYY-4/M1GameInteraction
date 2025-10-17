using Atlantis.Game;
using Atlantis.Menus;
using Atlantis.Menus;
using Atlantis.Scene;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Windows;
using System.Windows.Controls;

namespace Atlantis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Page _page;
        private Canvas _canvas;
        private Grid _grid;
        private SettingsMenu _menu;
        private string _image;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string BackgroundImage
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(BackgroundImage));
            }
        }

        public MainWindow()
        {
            // BOX2D ASSERTION: result.distanceSquared > 0.0f, C:\repos\box2d\src\manifold.c, line 848

            InitializeComponent();
            DataContext = this;
            BackgroundImage = "/Assets/MenuBackground.png";

            PushPage(new MainMenuPage(this));

            var f = App.Current.FindResource("MainFont");

            // Page is not added to PageHistory, because you shouldn't be able to leave MainMenu

            // Expanding upon the designer to use it for Game Design:
            //  - To design a level 'elements' need to be placed
            //  - Are bindings relevant? Answer: For rendering, physics, game features. Probably not because they have a specific context/purpose which isn't very expandable. (It's not 1:1 but it does not expand well when the code grows bigger)
            //  - The Designer provides: Dragging base WPF elements. Which could be a powerfull tool if it can be used to design levels.
            // Unity?: GameObject rendering/physics in Frame done for user, Components can be added to a GameObject. Complex and difficult to fully implement.
            // UE?: I don't know, Actors and components, but also requires complex infrastructure (code routing) to fully implement.
            // Tags: Code/Classes can be registered to handle certain tagged objects, this way the if if if chain is replaced with a loop matching the tags and invoking callbacks.
            //       Simple to implement and easy to use, however some code might want to know of other tagged objects, button wants to know it's doors or other, which causes a dependency problem.
            //       If a button is loaded before it's door, it either can't initialize or would have to listen for added objects. Not desirable.
            // 
            // Result: Unity GameObject used as inspiration for XAML hierarchy. It allows defining a scene tree.
            //         UserControls allow visually rendering the scene
            //         Tags denied because the concept comes from lua and does not fit the c# language or wpf.

            //{ // ground, this will probably become a 'border' around the level with 4 box shapes
            //    b2BodyDef def = B2Api.b2DefaultBodyDef();
            //    def.position = new Vector2(0, 0);

            //    b2BodyId groundId = B2Api.b2CreateBody(WorldId, def);
            //    b2Polygon poly = B2Api.b2MakeBox(1e4f, 1.0f);

            //    b2ShapeDef groundShapeDef = B2Api.b2DefaultShapeDef();
            //    b2ShapeId shapeId = B2Api.b2CreatePolygonShape(groundId, groundShapeDef, poly);
            //}
        }

        public List<Page> PageHistory { get; } = new List<Page>();

        public void GoBack()
        {
            Content = PageHistory[PageHistory.Count - 2];

            if (PageHistory.Count != 1)
            {
                PageHistory.RemoveAt(PageHistory.Count - 1);
            }

            Trace.WriteLine("AfterBack: " + string.Join(", ", PageHistory));
        }

        public void GoBackToType<PageType>()
        {
            for (int i = PageHistory.Count - 1; i >= 0; i--)
            {
                if (PageHistory[i] is PageType)
                {
                    GoBackToIndex(i);
                    break;
                }
            }
        }

        private void GoBackToIndex(int index)
        {
            Page page = PageHistory[index];
            if (index != 0)
            {
                PageHistory.RemoveRange(index, PageHistory.Count - index);
            }
            Content = page;
        }

        public void PushPage(Page page)
        {
            page.ClearValue(Page.HeightProperty);
            page.ClearValue(Page.WidthProperty);

            PageHistory.Add(page);
            Content = page;

            Trace.WriteLine("AfterPush: " + string.Join(", ", PageHistory));
        }

        public void ChangeBackground(string imageFilePath)
        {
            BackgroundImage = imageFilePath;
        }

        protected void OnPropertyChanged(string propertyName)
        {

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
    }
}


