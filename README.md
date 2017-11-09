# Handmade Ray in dotnet core

This is my dotnet core implementation of Casey Muratori's ray tracer, as seen in the Handmade Hero video series.

## Episode notes

### Episode 00 - Making a Simple Raycaster
https://www.youtube.com/watch?v=ZAeU3Z0PmcU

For this episode I followed Casey's implementation pretty closely. Instead of writing my own bitmap writing code, I used the `CoreCompat.System.Drawing.v2` package. For vectors I'm using `System.Numerics`.

### Episode 01 - Multithreading
https://www.youtube.com/watch?v=ZAeU3Z0PmcU

Instead of a work queue, I used dotnet's `Parallel.For` to render the tiles in parallel. One thing I learned while working on this is that an instance of the `Random` class is not thread safe :)

## Latest render

![Output][HandmadeRay/output.bmp]