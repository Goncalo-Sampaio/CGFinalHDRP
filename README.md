# Final Project - CG

## **Gonçalo Sampaio | 22400599**

### Waterfall, Water, Waves and Mist Shaders

I'm using a gameplay recording of the game Monster Hunter Rise as reference.
I will try to identify what type of shaders are being used and create them eventhough they won't be as "realistic" as in the recording.
There's a lot of ways to create all the effects like shader code, shadergraph, particle systems and VFX graph. I will be trying to create all the effects using only shader graph in URP.

![MonsterHunterWaterfall](/MonsterHunterWaterfall.mp4)

I started by trying to identify the different present shaders:
Waterfall
Water Waves
Waterfall Impact
Water Intersection with the rocks
Water mist near the impact

I decided to start by searching for references for each type of shader.

---

### WATERFALL

MAIN REFERENCE: https://www.youtube.com/watch?v=yJ0NRr-DdYU&t=1146s&ab_channel=GabrielAguiarProd.

After watching all the collected references for the waterfall, i decide to use Unity Shader Graph - Waterfall Effect Tutorial as reference for the shader.
The most realistic waterfalls that i found were all made using particle systems instead of shaders (ex: https://www.youtube.com/watch?v=XhSp8nFLUi4&ab_channel=mrwasd)

So i will be making a waterfall that is not like the one on the recording but its more aligned with the objectives of this project.

We start by adding a UV node and a Time node because we will scroll a texture on the object to simulate water movement.
Then we connect the Time node to a Multiply node, along with a float property to multiply the speed that the water moves.

![Waterfall1](/Prints/Waterfall1.png)

I then connected the multiply node to an Add Node along with the UV node to move the UVs over time. Then connected the Add node to a Voronoi Node to move this texture.

After that, i noticed that the texture was moving diagonally. That is because i initially used a Float property to multiply the movement.
So i changed the movement multiplier to a Vector2 so i can make the texture move vertically by just changing Y's value and leaving X as 0.

![Waterfall2](/Prints/Waterfall2.png)

To have a more different pattern that moves over time, I connected the Time Node to a different Multiply Node along with a new Float Property. Then connected the Multiply Node to the Angle Offset of the Voronoi Node.
This makes each cell in the voronoi move over time independently, on top of the basic vertical voronoi movement.

By creating another Float Property and connecting it to the Voronoi "Cell Density" entry, we can change the scale of the details in the Voronoi.

If we connect the voronoi node and a float property to a Power Node, we can change the intensity of the details.

To add some colour to the shader, we can multiply the Power Node with a Colour Property.

The reference video was using a vertex colour multiplied by the previous Multiply Node, but i was having trouble understanding why was it being used.
I tried using it and not using it and it looked exactly the same.
At first i was only using a unity plane to test the shader and i thought that maybe that was the issue, so i decided to create a simple "waterfall" shape in blender and export it to unity.

After fixing the waterfall's UVs, i tried to see the difference again on the shader, but again, it looked exactly the same.
I looked for unity's documentation on the Vertex Color Node but it doesn't say anything useful.
Then i looked for an explanation of this Node on this site: https://danielilett.com/2021-05-20-every-shader-graph-node/
From what i understood, i need the set up vertex color data beforehand on the modelling software, which i didn't do. So i just assumed that using this Node would be useless to me and removed it from the graph.

![Waterfall3](/Prints/Waterfall3.png)

To reduce the dark spots/increase the brightness, i added a Remap Node after the Power Node and changed the values to my liking. The Remap Node will take the input and transform the minimum and maximum to new values, allowing me to make the dark spots darker or brighter. Same for the white parts of the voronoi texture.

Before continuing to follow the video, i wanted to change the scale of the texture so i connected the UV Node to a Tiling and Offset Node, and also added a Vector2 property to give me better control over the scalling.
I wanted the texture to be a bit more stretched on the Y axis, so i set the Y Tiling to 0.2

![Waterfall4](/Prints/Waterfall4.png)

To add a bit of a "foam" effect at the end of the waterfall, I use a Split Node to get the green colour of the UV Node, which gives me access to the textures's vertical information.
By inverting the green channel with a One Minus Node, we get a gradient that is brighter on the bottom. If we use a Power Node here, we can intensify the gradient.
Then we multiply the Power Node with Simple Noise Node and then we can use a Remap Node to change the brightness of the foam.
I also connected the movement part of the graph to the UV channel on the Simple Noise to also make it move over time.

![Waterfall5](/Prints/Waterfall5.png)

To have actual movement on the object, i connected the Voronoi result to a Multiply Node along with a Float Property to control the displacement, then connected the Multiply to an Add Node along with a Position Node with Object Space.
The fact that the position node is set to Object space, makes the vertex displacement happen relatively to the object's pivot.

![Waterfall6](/Prints/Waterfall6.png)

To add more foam to the waterfall, i changed the shader to transparent and enabled alpha clipping.
Then connected the sum of both noises a One Minus Node, to invert them and connected it to the Alpha Clipping channel. Also created a Float property to control the alpha, connected it to a One Minus Node and then connected it to the alpha channel.
At first i didn't restrict this property and was able to change to negative alpha values and values over 1, which were causing weird colors and shapes. Alpha is supposed to be between 0 and 1, so using a slider with these values is probably the best practice.

I duplicated the waterfall, to act as foam, duplicated the material and changed the values to my liking.

![Waterfall7](/Prints/Waterfall7.png)

---

### WATERFALL IMPACT

MAIN REFERENCE: https://www.youtube.com/watch?v=yJ0NRr-DdYU&t=1146s&ab_channel=GabrielAguiarProd.

I started by creating an object in blender to apply the water impact shader, and imported it to Unity. I also created a seamless texture to use in the shader.

![Impact1](/Prints/Impact1.png)

Then i created a new shader with a very similar basic setup to the waterfall shader with a slight difference: instead of having a noise, it has a Sample Texture 2D Node to which we connect a Property that will be the wave texture that i created.

![Impact2](/Prints/Impact2.png)
![Impact3](/Prints/Impact3.png)

To not have such hard edges on the waves, i had to add another 2d texture and multiply it by the wave texture. This new texture is white on the center and has a gradient that turns black towards the edges.
I was having trouble making the alpha work correctly as it wasn't following the alpha from the second texture as some parts were disappearing abruptly.
After some time and testing different textures, i realized that that the textures were disappearing when the alpha is bellow a certain level.
I had turned on alpha clipping and it was indeed clipping the alphas under 0.5, so i turned it off and everything worked normally.

In order to add a little bit of distortion to the main texture, we can connect the UV Node to a Lerp Node and to another Add Node to which we can connect a multiplication of the Time Node and a float property to control the speed at which the noise will move.
Then we connect this Add Node to a Voronoi Node which we will then connect to another Add Node to add the initial UVs and connect it to the B channel of the Lerp.
By connecting a float property to the Lerp we can control the interpolation between these 2 UVs, meaning that we can change the shader to have no distortion and use 100% of the original UVs, be completely distorted by the Voronoi Noise, or use a percentage of both.
Note: This property should be a slider between 0 and 1 because its an interpolation that goes from 0 to 100% of one or the other UV. Allowing other values might cause weird results.

Lerp explanation: https://danielilett.com/2021-05-20-every-shader-graph-node/

![Impact5](/Prints/Impact5.png)

There's a weird cut on the texture but its probably related to the way i made the object's UVs on Blender, or maybe the texture isn't as seamless as i tried to make it. Anyway, it won't be visible at the end, as it will be under the waterfall.

![Impact4](/Prints/Impact4.png)

The waves are still looking too fake. One of the solutions to deal with this, it's to dissolve the wave texture.
We can do that by multiplying the main texture by the previously created Voronoi and connect it to a new Lerp Node and add a property to control the interpolation.
Then we connect the Lerp Node to a Power Node with a float property to control how much we want to dissolve the texture.

The Power Node returns the first input raised to the second input. (https://danielilett.com/2021-05-20-every-shader-graph-node/)

![Impact6](/Prints/Impact6.png)

The reference video suggests to create a particle system to have more variety with different rotations, for example.
I won't do it, because if i use different rotations, the flaw that was initially hidden under the waterfall, might become visible.
I might create a particle system later to give it different colors, durations, etc.

BONUS:

While i was trying to see if the flaw was related to something on the shader, i connected the initial UVs to the color channel and got this cool, almost radioactive effect.
Might be useful for other projects.

![Impact7](/Prints/Impact7.png)

---

### WATER

MAIN REFERENCE: https://www.youtube.com/watch?v=DpXPhGeCqus&list=PL7pZIxQeCqI2Dddld7fLkst1yWzdGAK_K&ab_channel=BinaryLunar

For the water, it seems that it doesn't have any sort of transparency, refraction or a depth mask.
It looks like its just displacement and moving the texture over time.

I started by multiplying a Time Node with a Vector2 Property to control the movement. Then added it to the Position: Object Node to get access to the vertexes.
To add a more "wavey" noise, I connected the Add Node to a Gradient Noise Node with a Property to control the level of detail and then multiplied it by another property control the height of the waves.

![Water1](/Prints/Water1.png)

Then i multiplied it with a Normal Vector Node to move the vertexes along their normals. And then i Added this to a Position: Object Node to apply the changes to the original position of each vertex, and connected this to the Vertex Position channel.

![Water2](/Prints/Water2.png)

(I Stopped following the reference here)

I Changed the first Position Node to a UV Node because initially, the plane wasn't moving how i wanted.

![Water3](/Prints/Water3.png)

After that, i wanted to give the plane some color but i wanted the color to be darker betweeen waves, so i connected the noise to a Remap Node and increased the minimum to 0.5 to brighten the darker parts of the Noise.
Then multiplied this result with a Color Property and connected the output to the Color Channel.

![Water4](/Prints/Water4.png)

I overlapped all the shaders I made so far, to see how they all looked together. After adjusting the water color, I decided to leave it the way it is, for now.

![Water5](/Prints/Water5.png)

---

### EDGES

NO REFERENCE

For the edges I thought that maybe creating some sort of inverted Impact Shader and scaling it up would work.
I started by duplicating the impact object and shader, and creating a new material with the new shader.
Then i inverted the velocities, both the waves and the voronoi noise, to make the waves start on the outside and move to the center.

This is how the original Impact Shader is:

![Edges2](/Prints/Edges2.png)

After that, I inverted the alpha Mask using a One Minus Node.
Unfortunately, this also inverted the colors, which wasn't intended.

![Edges1](/Prints/Edges1.png)

So, instead of connecting the Mask to the One Minus Node before multiplying, I let it stay connected directly to the multiplication with the other texture, to not affect the colors.
And i added a different multiplication for the Alphas, so only they take the One Minus into account:

![Edges3](/Prints/Edges3.png)
![Edges4](/Prints/Edges4.png)

I'm getting closer to the result i want but i need to fix the texture used on the Alpha Mask as the one used for the impact, has transparency at the inner and the outer faces of the circle plane object.
I need it to only have transparency at the outer ring.
After creating another texture closer to what i want, this is the result:
(Also disabled Impact Object for a clearer view)

![Edges5](/Prints/Edges5.png)

Now the next step is to find a way to reduce the scaling because, as I scaled the object up, the waves also got bigger, but i want them smaller.
I added a Tiling Node to the UVs and created a Vector2 property connected to the tiling. This allows me to change the scale of the UVs, which i did to scale the waves down.

![Edges6](/Prints/Edges6.png)

The edges might still be a lasting for too long, but i need to try everything together at the end, to see if i want to change something.

NOTE: A possible solution for this - https://discussions.unity.com/t/shader-graph-texture-scaling/843469
If I scale the Alpha Mask up, the waves will probably disappear sooner.

---

### MIST
  
After looking at all the references, none of them were fitting what i needed. Some were for HDRP (i was using URP for the other shaders), others were using VFX graph and/or particles.
I tried replicating the 2 shaders in this reference:
https://cyangamedev.wordpress.com/2019/12/05/fog-plane-shader-breakdown
But i couldn't get them to work, even after enabling the depth texture on the URP Asset.

Then i replicated this one:
https://www.youtube.com/watch?v=iF4ZKLtYZ3I&t=10s&ab_channel=SimblendGames

Eventhough it kind of works, it's very noticeable where it ends and it only looks good if its a type of fog that affects the whole level.
I need something more localized.

![Mist1](/Prints/Mist1.png)

There's also this one but it's very "cloudy" and will also be noticeable where it ends: https://www.youtube.com/watch?v=Y7r5n5TsX_E&t=458s&ab_channel=RomanPapush

This one would be perfect but it's a shader for HDRP: https://www.youtube.com/watch?v=En-VjBjto_U&ab_channel=BenCloward

I was about to give up but then i decided to create a new HDRP project and recreate all the shaders in HDRP.
And so, i will be following the HDRP Mist shader reference.

I Started by creating a Local Volumetric Fog Object and changed the Mask Mode to Material so the shader can control the mist.

I Created a new Fog Volume SHader Graph.

I created a Position Node in Object Space and connected it to a 2D Texture. Then inserted a Swizzle Node between them with a XZ Mask to project the texture from the top.
To control the tiling, I multiplied the swizzle by a Float property.

To make the mist move, I created a float property to control the speed and a Vector2 property to control the direction and multiplied them. Then multiplied the result by Time and Added the result the to tiling.

![Mist2](/Prints/Mist2.png)

Then, to make the mist show up on the bottom of the volume, we connect the red channel of the texture to an Inverse Lerp Node.
To use the height of the Mist, I needed to add a UV Node, Split it and connect the green channel to a multiply along with a float property to control how tall or how flat the the mist is.
And I connected the result to the Interpolation value of the Inverse Lerp.

Now to get a value between 0 and 1, I connected the Inverse Lerp to a Saturate Node and then to a One Minus to invert the values.
Finally, I multiplied the result by a Float property that will control the Strenght of the mist.

![Mist3](/Prints/Mist3.png)

To add a bit of detail, I duplicated the nodes that handle the speed, the direction, the tiling and the texture, but with a negate Node after the direction property, to give it an opposite move direction to the main texture.
And connected the result to an Inverse Lerp Node at the the end of the shader with the main texture connected to the Interpolation Channel.
This will return the necessary interpolation between 0 (channel A) and the detail texture (channel B) to get the Main texture.

![Mist4](/Prints/Mist4.png)

Lastly, i created a color Node and connected it to the Base Color.
After adjusting all the values i got this result:

![Mist5](/Prints/Mist5.png)

To finalize the project, i created a quick cliff in blender, searched for some cliff textures, placed everything in the scene and gave the player some movement.

![Final](/Prints/Final.png)

In the end, i'm not happy with the result but at least i learnt some things.


## REFERENCES

Shader Graph Nodes: https://danielilett.com/2021-05-20-every-shader-graph-node/

### Impact
- https://www.youtube.com/watch?v=yJ0NRr-DdYU&t=1146s&ab_channel=GabrielAguiarProd.
- https://www.youtube.com/watch?v=uOhWT6TxZgE&t=10s&ab_channel=GabrielAguiarProd.
- https://www.youtube.com/watch?v=_H8gBKGKbnU&ab_channel=GabrielAguiarProd.

### Mist
- https://cyangamedev.wordpress.com/2019/12/05/fog-plane-shader-breakdown
- https://www.youtube.com/watch?v=En-VjBjto_U&ab_channel=BenCloward
- https://www.youtube.com/watch?v=H5QZhChfa1I&ab_channel=GabrielAguiarProd. (VFX graph)
- https://www.youtube.com/watch?v=Y7r5n5TsX_E&t=458s&ab_channel=RomanPapush
- https://www.youtube.com/watch?v=iF4ZKLtYZ3I&t=10s&ab_channel=SimblendGames

### Waterfall
- https://www.youtube.com/watch?v=DIE3qfCGXl8&t=2s&ab_channel=MinionsArt
- https://www.youtube.com/watch?v=yJ0NRr-DdYU&t=1145s&ab_channel=GabrielAguiarProd.
- https://www.youtube.com/watch?v=_H8gBKGKbnU&ab_channel=GabrielAguiarProd.
- https://www.youtube.com/watch?v=uOhWT6TxZgE&t=10s&ab_channel=GabrielAguiarProd.
- https://www.youtube.com/watch?v=4X9ak_SjNwU&ab_channel=GameSlave

### Edges
- https://www.patreon.com/posts/30490169
- https://www.youtube.com/watch?v=uOhWT6TxZgE&t=10s&ab_channel=GabrielAguiarProd. (The edges also look like they are the opposite of impact)
- https://www.youtube.com/watch?v=MHdDUqJHJxM&ab_channel=BinaryLunar
- https://www.youtube.com/watch?v=jRuCQnp78gk&ab_channel=GiusCaminiti

### Water:
- https://www.youtube.com/watch?v=DpXPhGeCqus&list=PL7pZIxQeCqI2Dddld7fLkst1yWzdGAK_K&ab_channel=BinaryLunar
- https://www.youtube.com/watch?v=DIE3qfCGXl8&t=2s&ab_channel=MinionsArt
- https://www.youtube.com/watch?v=MHdDUqJHJxM&ab_channel=BinaryLunar
- https://www.youtube.com/watch?v=S4RrVKBEaDw&ab_channel=eleonora
- https://www.youtube.com/watch?v=FlE8e1JwVzs&ab_channel=GabrielAguiarProd.
- https://www.youtube.com/watch?v=kgXeo2SRDd4&ab_channel=PolyToots
- https://www.youtube.com/watch?v=FbTAbOnhRcI&ab_channel=PolyToots
