using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace KFF.Tests
{
    [TestClass]
    public class PathSegmentTests
    {
        public static IEnumerable<object[]> GetValidPathSegmentNames
        {
            get
            {
                yield return new object[] { "name" };
                yield return new object[] { "name1234" };
                yield return new object[] { "name_with_underscores" };
            }
        }
        
        public static IEnumerable<object[]> GetValidPathSegmentIndices
        {
            get
            {
                yield return new object[] { 0 };
                yield return new object[] { 1 };
                yield return new object[] { 69 };
            }
        }

        //
        //      TESTS
        //

        [DynamicData("GetValidPathSegmentNames")]
        [TestMethod]
        public void NamedPathSegment(string name)
        {
            PathSegment pathSegment = new PathSegment( name, new int[0] );

            Assert.IsTrue( pathSegment.name == name );
            Assert.IsTrue( pathSegment.index == -1 );
            Assert.IsTrue( pathSegment.destination == DataStructures.ObjectType.Tag );
            Assert.IsTrue( pathSegment.direction == PathDirection.Forward );
        }

        [DynamicData( "GetValidPathSegmentIndices" )]
        [TestMethod]
        public void PathSegmentWithPlaceholder(int index)
        {
            PathSegment pathSegment = new PathSegment( "{0}", new int[] { index } );

            Assert.IsTrue( pathSegment.name == index.ToString( Syntax.numberFormat ) );
            Assert.IsTrue( pathSegment.index == index );
            Assert.IsTrue( pathSegment.destination == DataStructures.ObjectType.Payload );
            Assert.IsTrue( pathSegment.direction == PathDirection.Forward );
        }

        [TestMethod]
        public void BackwardsPathSegment()
        {
            PathSegment pathSegment = new PathSegment( "<", new int[0] );

            Assert.IsTrue( pathSegment.name == null );
            Assert.IsTrue( pathSegment.index == -1 );
            // pathSegment.destination undefined
            Assert.IsTrue( pathSegment.direction == PathDirection.Backward );
        }
    }
}
