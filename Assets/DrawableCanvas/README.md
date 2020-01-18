# DrawableCanvas

A asset to add a dynamic paintable texture with collision detection.

### Installing

You must include the library at the start of any file that use it.

```csharp
using DrawableCanvas;
```

You must enable read and write on each texture you use as a brush.
To avoid bug make sure the pixel per unit of your brush is the same as your Canvas2D.

For more precise application check out the examples.

## Functions

### Canvas2D

```csharp
void applyTextureOnPoint(Texture2D source, Vector2 point)
```
Paint the canvas at the point indicated.

```csharp
void applyTextureOnPoint(Texture2D source, Vector2 point, Bounds filter)
```
Paint the canvas at the point indicated only if it's contained in the bounds.

```csharp
void removeTextureOnPoint(Texture2D source, Vector2 point)
```
Remove the paint in the form of the texture linked.( transparent pixel are not taken in to account).

```csharp
void setColor(Color c)
```
Set the color of the canvas.

```csharp
Vector2 worldToTexture(Vector2 point)
```
Convert a point from world position to texture position.

```csharp
Vector2 textureToWorld(Vector2 point)
```
Convert a point from texture position to world position.

### ColliderBuilder

```csharp
void addShapeCollider(List<Vector2> points, Vector3 position)
```
add a polygon to the builder at the position wanted.

```csharp
void removeShapeCollider(List<Vector2> points, Vector3 position)
```
remove a polygon to the builder at the position wanted.

```csharp
void addFilter(Vector2[] points);
void removeFilter(Vector2[] points);
```
add or remove a polygon to the filter of the collider.

```csharp
void addFilter(Vector2[] points, int n, Vector3 scale , Vector3 position);
void removeFilter(Vector2[] points, int n, Vector3 scale, Vector3 position);
```
add or remove a polygon to the filter of the collider and scale the point of the polygon to the size of the object

## Filter

For the collision builder if you want to add a filter depending on the filter you have two choices:

-if you use a Composite collider you can just send each path of the collider with
```csharp
 Vector2[] points = new Vector2[filter.GetPathPointCount(i)];
            filter.GetPath(i, points);

colliderBuilder.addFilter(points);
```

because the points are already scaled to the world position.

But for any other type of collider use

```csharp
colliderBuilder.addFilter(collider.points, filter.GetTotalPointCount(),filter.transform.lossyScale,filter.transform.position);
```

because the points are set to local position so you need to rescale them.

## Authors

* **Steags**

## Acknowledgments

* Check out Clipper lib : http://www.angusj.com/delphi/clipper.php
