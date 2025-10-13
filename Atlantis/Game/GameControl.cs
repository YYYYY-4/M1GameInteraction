using Box2dNet.Interop;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Atlantis.Box2dNet;

namespace Atlantis.Game
{
    /* Goal: A element that can be defined like the following and appear in the xaml editor
     * Reason: It could act as the base definition for physics/shapes/bodies to a world
     *  
     * First idea:
     * <Body>
     *      <Rect Angle, Offset, />
     *      <Circle Radius, Offset, Angle/>
     * </Body>
     * 
     * 
     * Second idea psuedo code:
     * Reasoning: A b2Body can have multiple shapes, therefore it must be possible to attach multiple shapes
     * 
     * <MainWindow>
     *  -- Scene Root 
     *  -- After recursively loading children from Root, the GameControl's are translated to global space and parented to the root canvas, don't use .Children
     *  -- GameControl inherits UserControl from WPF
     *  -- A GameControl always has a b2Body, it's a 1:1, shapes within the GameControl.Content (which can forward to a canvas) are added to the GameControl
     *  -- Any sub-canvas is added as GameChild? or base GameControl?
     *  -- Any GameControl within another is considered a seperate instance
     *  
     *  <Canvas> - root
     *      <Player inherits GameControl> 
     *         <Canvas> - b2BodyId
     *            
     *         </Canvas>
     *      </Player>
     *      
     *      <Button />
     *      
     *      - Wall defines itself as a Rectangle at root
     *      <Wall Position />
     *      <Wall Position />
     *      <Wall Position />
     *  </Canvas>
     * 
     * </MainWindow>
     */

    /// <summary>
    /// The base class for (physics) things in the game.
    /// They are many.
    /// </summary>
    public class GameControl : UserControl
    {
        // Control ID primarly for debugging (for now)
        public long CID = 0;

        // Initialized by World loading
#if DEBUG
        private b2BodyId _body;

        public b2BodyId Body
        {
            get
            {
                if (!DesignerMode)
                {
                    Debug.Assert(_body.IsValid());
                }
                return _body;
            }
            set 
            {
                _body = value;
            }
        }
#else
        public b2BodyId Body;
#endif


        public RotateTransform Rotate;
        public TranslateTransform Translate;

        // Initialized by World loading
        // [WPF Shape] = b2ShapeId
        //public Dictionary<Shape, b2ShapeId> ShapeIds;

        public List<GameShape> Shapes;

        // utility getter for first shape
        public GameShape Shape0 => Shapes[0];

        // When elements define properties in code-behind based on changes at runtime they should check if this is true.
        // To properly load a scene it is initally true, until after OnStart it is true/false based on actual design mode.
        // For example inner elements base some properties on the parent size, initially that should update, and within the designer it should actively update.
        protected bool DesignerMode = true;

        public GameScene Scene;

        public bool IsKeyDown(Key key)
        {
            return Scene.IsKeyDown(key);
        }

        // returns 0/1
        public int IsKeyDown01(Key key)
        {
            return Scene.IsKeyDown01(key);
        }

        public GameControl()
        {
        }

        public BodyDef? FindWPFBodyDef()
        {
            var def = BodyDef.GetBodyDef(this);
            if (def == null && Content is FrameworkElement content && content is not GameControl)
            {
                def = BodyDef.GetBodyDef(content);
            }
            return def;
        }

        public virtual void ModifyBodyDef(ref b2BodyDef bodyDef)
        {
            // Check if this has a BodyDef, if not also check Content unless Content is a GameControl
            var def = FindWPFBodyDef();
            def?.ApplyBodyDef(ref bodyDef);
        }

        public virtual void ModifyShapeDef(ref b2ShapeDef shapeDef)
        {
            // Check if this has a ShapeDef, if not also check Content unless Content is a GameControl
            // A ShapeDef's properties defined on the Body are overwritten by a ShapeDef of a specific Shape.
            var def = ShapeDef.GetShapeDef(this);
            if (def == null && Content is FrameworkElement content && content is not GameControl)
            {
                def = ShapeDef.GetShapeDef(content);
            }
            def?.ApplyShapeDef(ref shapeDef);
        }

        public virtual void OnStart()
        {
            DesignerMode = DesignerProperties.GetIsInDesignMode(this);
        }

        public virtual void OnUpdate(float dt)
        {
        }

        public virtual void OnSensorStart(GameShape sensor, GameShape visitor)
        {
        }

        public virtual void OnSensorEnd(GameShape sensor, GameShape visitor)
        {
        }
    }
}
