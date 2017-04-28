# TODO - MVVM

## Navigation Containers

Add overlays on modal presentation. Should have a fadein semi-transparent black background behind the window that is pushed in.

## Navigation Gesture - OK

Reimplement navigation gesture 

## Storage provider for values - OK

Persistent storage

MvvmApp.Load<int>("somekey")
MvvmApp.Save<int>("somekey", value)

## Dictionary provider for values - OK

Non-persistent storage, use for config values

MvvmApp.Get<int>("somekey")
MvvmApp.Set<int>("somekey", value)

# Animation 

## Interpolator - OK
Interpolate between two animations using 0 .. 1.0 as values:

animation.Run(animation)
animation.Interpolate(animation, 0.5)

## Animatable Layouts in Xamarin.Forms
Create a layout that can be animated. What we need is to do the following:

### RelativeLayout

We animate changes to the rect of controls in the layout using the Layout(Rectange) method. By creating an interpolator that can interpolate between two constraints from a function of time (0.0 -> 1.0) we can interpolate layouts. This works and seems to be fast enough.

The challenge here is to find a way to define constraints in the layout that can be used both for defining multiple states (start, stop positions for a control f.ex), and when interpolating along time.

# Gesture recognizer - OK
Add support for cancelling og taking touch.