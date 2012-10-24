﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    /// <summary>
    /// Builds input for Position/Normal/Texcoord triangles
    /// </summary>
    public class SimpleMeshFragmentInputBuilder : IMeshFragmentInputBuilder
    {
        public FragmentInput CalculateInput(IMesh mesh, Matrix world, MeshRaycastResult raycast)
        {
            var input = new FragmentInput();
            input.Normal = raycast.Vertex2.Normal * raycast.U + raycast.Vertex3.Normal * raycast.V +
                           raycast.Vertex1.Normal * (1 - raycast.U - raycast.V);

            //input.Normal =
            //    Vector3.Normalize(-Vector3.Cross((raycast.Vertex1.Position - raycast.Vertex2.Position),
            //                                    (raycast.Vertex1.Position - raycast.Vertex3.Position)));

            //input.Normal = raycast.Vertex1.Normal;
            input.Normal = Vector3.Normalize(input.Normal); // Renormalize!
            input.Texcoord = raycast.Vertex1.Texcoord * raycast.U + raycast.Vertex2.Texcoord * raycast.V +
                           raycast.Vertex3.Texcoord * (1 - raycast.U - raycast.V);
            //TODO: perspective correction

            //input.Normal = raycast.Vertex2.Normal;

            //input.Diffuse = new Color4(0.2f, 0.8f, 0.3f);
            input.Diffuse = new Color4(0.8f, 0.8f, 0.8f);
            input.SpecularColor = new Color4(1, 1, 1);
            input.SpecularPower = 15;
            input.SpecularIntensity = 2;
            //input.Diffuse = new Color4(raycast.U, raycast.V, 0);
            
            return input;
        }
    }
}
