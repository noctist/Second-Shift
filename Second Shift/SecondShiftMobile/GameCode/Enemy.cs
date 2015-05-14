using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SecondShiftMobile.Networking;

namespace SecondShiftMobile
{
    public class Enemy : CombatObj
    {
        public Enemy(Game1 Doc, TextureFrame tex, float x, float y, float z)
            :base(Doc, tex, x, y, z)
        {
            Attackable = true;
            IsNetworkCapable = true;
            Faction = SecondShiftMobile.Faction.Evil;
        }
        public override void PeerConnected()
        {
            if (NetworkManager.SocketRole == SocketRole.Host)
            {
                SendHostOrClientRequest(true);
            }
            base.PeerConnected();
        }
        public override void Update()
        {
            base.Update();
        }
        protected override bool AttackedOverride(Attack attack, Obj obj, Rectangle AttackBox, Rectangle intersection)
        {
            //if (!(obj is Enemy))
            {
                return base.AttackedOverride(attack, obj, AttackBox, intersection);
            }
        }
        protected override void Die(Attack attack, Obj obj, Rectangle intersection)
        {
            base.Die(attack, obj, intersection);
            Remove();
        }
        
    }
}
