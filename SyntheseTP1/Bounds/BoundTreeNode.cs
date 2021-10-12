using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;
using Triangle = SyntheseTP1.Shapes.Triangle;

namespace SyntheseTP1
{
    class BoundTreeNode : BoundTreeNodeBase
    {
        public BoundingBox boundLeft;
        public BoundingBox boundRight;

        public BoundTreeNodeBase leftTree;
        public BoundTreeNodeBase rightTree;

        public const int MinTriangles = 6;

        public BoundTreeNode(List<Triangle> triangles, BoundingBox bounds)
        {
            Vector3 size = bounds.max - bounds.min;
            int largestAxis = size.X > size.Y ? (size.X > size.Z ? 0 : 2) : (size.Y > size.Z ? 1 : 2);

            List<Triangle> sortedTris = triangles.OrderBy(t => t.center.GetComponent(largestAxis)).ToList();

            int halfTris = sortedTris.Count / 2;

            List<Triangle> leftTris = sortedTris.GetRange(0, halfTris);
            List<Triangle> rightTris = sortedTris.GetRange(halfTris, sortedTris.Count - halfTris);

            boundLeft = new BoundingBox(leftTris);
            boundRight = new BoundingBox(rightTris);

            if (leftTris.Count <= MinTriangles)
                leftTree = new BoundTreeLeafMesh(leftTris);
            else
                leftTree = new BoundTreeNode(leftTris, boundLeft);

            if (rightTris.Count <= MinTriangles)
                rightTree = new BoundTreeLeafMesh(rightTris);
            else
                rightTree = new BoundTreeNode(rightTris, boundRight);
        }

        public override Hit Intersect(Ray ray)
        {
            float? boundHitLeft = boundLeft.Intersect(ray);
            float? boundHitRight = boundRight.Intersect(ray);

            if (boundHitLeft.HasValue)
            {
                if (boundHitRight.HasValue)
                {
                    bool leftIsCloser = boundHitLeft.Value < boundHitRight.Value;

                    BoundTreeNodeBase closeTree = leftIsCloser ? leftTree : rightTree;
                    BoundTreeNodeBase farTree = leftIsCloser ? rightTree : leftTree;
                    float? farBoundHit = leftIsCloser ? boundHitRight : boundHitLeft;

                    Hit closeHit = closeTree.Intersect(ray);
                    if (closeHit == null)
                        return farTree.Intersect(ray);

                    if (closeHit.distance < farBoundHit.Value)
                        return closeHit;

                    Hit farHit = farTree.Intersect(ray);
                    if (farHit == null)
                        return closeHit;

                    if (farHit.distance < closeHit.distance)
                        return farHit;

                    return closeHit;
                }

                return leftTree.Intersect(ray);
            }

            if (boundHitRight.HasValue)
            {
                return rightTree.Intersect(ray);
            }

            return null;
        }
    }
}
