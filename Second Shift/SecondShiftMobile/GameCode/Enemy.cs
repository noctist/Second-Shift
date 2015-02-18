using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SecondShiftMobile
{
    public class Enemy : CombatObj
    {
        
        
        
        public Enemy(Game1 Doc, TextureFrame tex, float x, float y, float z)
            :base(Doc, tex, x, y, z)
        {
            Attackable = true;
            Gravity = 0.8f;
            
        }
        public override void Update()
        {
            
            base.Update();
        }
        public override bool Attacked(Attack attack, Obj obj, Rectangle AttackBox, Rectangle intersection)
        {
            //if (!(obj is Enemy))
            {
                return base.Attacked(attack, obj, AttackBox, intersection);
            }
        }
        protected override void Die(Attack attack, Obj obj, Rectangle intersection)
        {
            Remove();
            base.Die(attack, obj, intersection);
        }
        
    }
}
