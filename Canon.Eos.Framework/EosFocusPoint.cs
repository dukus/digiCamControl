using System.Drawing;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework
{
    public struct EosFocusPoint
    {
        internal static EosFocusPoint Create(Edsdk.EdsFocusPoint focusPoint)
        {
            return new EosFocusPoint
            {
                Bounds = new Rectangle {
                    X = focusPoint.rect.x,
                    Y = focusPoint.rect.y,
                    Height = focusPoint.rect.height,
                    Width = focusPoint.rect.width,
                },
                IsInFocus = focusPoint.justFocus != 0,
                IsSelected = focusPoint.selected != 0,
                IsValid = focusPoint.valid != 0,
            };
        }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in focus.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in focus; otherwise, <c>false</c>.
        /// </value>
        public bool IsInFocus { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected { get; private set; }        
    }
}
