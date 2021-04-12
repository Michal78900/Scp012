using CommandSystem;
using Exiled.API.Features;
using Mirror;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Extensions;

namespace Scp012
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Funi : ICommand
    {
        public string Command => "spawntablet";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "yes";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 3)
            {
                response = "You've fucked up something.";
                return false;
            }
            else
            {
                foreach(var pickup in UnityEngine.Object.FindObjectsOfType<Pickup>())
                {
                    if (pickup.itemId == ItemType.WeaponManagerTablet)
                    {
                        NetworkServer.Destroy(pickup.gameObject);
                    }
                }

                Player player = Player.Get((sender as PlayerCommandSender).ReferenceHub);

                Vector3 funiVector = new Vector3(float.Parse(arguments.At(0)), float.Parse(arguments.At(1)), float.Parse(arguments.At(2)));

                Pickup funi = Exiled.API.Extensions.Item.Spawn(ItemType.WeaponManagerTablet, 0, player.Position, Quaternion.Euler(funiVector));

                GameObject gameObject = funi.gameObject;

                gameObject.transform.localScale = new Vector3(0.5f, 2.5f, 2.5f);

                var rigidbody = gameObject.GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
                rigidbody.useGravity = false;
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.freezeRotation = true;
                rigidbody.mass = 100000;

                NetworkServer.UnSpawn(gameObject);
                NetworkServer.Spawn(gameObject);

                response = funiVector.ToString();
                return true;
            }
        }
    }
}
