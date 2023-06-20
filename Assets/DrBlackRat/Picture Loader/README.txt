VRC Picture Loader V1.3.0

This is a simple way to add an VRChat Image Downloader to your world!

What does it do?
- allows you to Automatically or Manually Download Pictures from the Web
- allows you to Reload Pictures Automatically
- allows you to keep the old Picture while redownloading it
- shows the the Download Progress (how many pictures have been loaded vs how many are there)
- shows an error when a picture couldn't be loaded
- let's you change texture settings
 - Mip Maps, Aniso Level & Filtering
- let's you set a Loading Texture while the Picture is Loading 
- let's you set an Error Texture if the Picture couldn't Load

Here is a video showcasing it:
https://youtu.be/xxTCXYHlpWg

There is a normal and Lite Downloader, which one should I use?
- The normal Downloader requires the Manager to function and is being controled by it
- The Lite Downloader is a simpler version which does not require the manager and works on it's own
- You can use both at the same time, but Lite Downloaders wont show up in the Manager

How to add it:
- I recommend to take a look at the Example Scene first

- you need to have TextMeshPro installed
- when you add the Prefab or open the Example Scene Unity prompts you with an install window, install every component
- after that reload the scene

Normal Setup:
- drag the Picture Loader prefab into your world
- add the Picture Downloader script to the Mesh / Raw Image you want to apply the downloaded picture to
- set the Material Properties or Raw Images you want the Texture to be applied to
- provide a direct link to the picture you want to download in the Url field

Lite Setup:
- add the Lite Picture Downloader script to the Mesh / Raw Image you want to apply the downloaded picture to
- set the Material Properties or Raw Images you want the Texture to be applied to
- provide a direct link to the picture you want to download in the Url field

This Asset was made by DrBlackRat
GitHub: https://github.com/DrBlackRat/VRC-Picture-Loader
Twitter: https://twitter.com/DrBlackRat
