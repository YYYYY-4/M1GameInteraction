using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using Atlantis.Game;
using Atlantis.Menus;
using Atlantis.Scene;

namespace Atlantis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameScene _scene;
        private Page _page;
        private Canvas _canvas;

        public MainWindow()
        {
            // BOX2D ASSERTION: result.distanceSquared > 0.0f, C:\repos\box2d\src\manifold.c, line 848

            InitializeComponent();
            //Content = ((Page)Content).Content;
            //_scene = new(this);

            //LoadScene<TestPage>();

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
        
        /// <typeparam name="T">Scene which inherits Page and defines a Canvas at it's root.</typeparam>
        public void LoadScene<T>() where T : Page
        {
            _scene?.Destroy();
            
            _page = new DemoLevel();
            _canvas = (Canvas)_page.Content; // GameCanvas is root element in TestPage
            
            Content = _canvas;
            _scene = new GameScene(this, _canvas);
        }

        static Matrix3x2 M3X2Inverse(Matrix3x2 m)
        {
            _ = Matrix3x2.Invert(m, out var result);
            return result;
        }

        static Matrix3x2 ToWorld(Matrix3x2 self, Matrix3x2 b)
        {
            return self * b;
        }

        static Matrix3x2 ToLocal(Matrix3x2 self, Matrix3x2 b)
        {
            return Matrix3x2.Identity;
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadScene<DemoLevel>();
        }

        private void Quit_Button_Click(object sender, RoutedEventArgs e)
        {
            // Terminates process and tells underlying process quit
            Environment.Exit(0);
        }
    }
}


