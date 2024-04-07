![BBBBBBBB+](https://github.com/DrBlackRat/VRC-Picture-Loader/assets/46327609/d407dab4-d36e-4723-8cd6-e41e9bcc6698)


## Here are the core features:
- Download Images from the Web
- Provides a UI for VRChat's Image Downloader
  - Light & Dark Mode
- Load them automatically on start & re download them after a specific amount of time
- Adjust texture settings:
  - Generate Mip Maps
  - Set Aniso Level & Filter Mode
  - Change the Wrap Mode 
- Apply the Texture to a Material
- Set multiple Material Properties it should be applied to
- Apply the Texture to UI Raw Images
- Set a texture that should be used while loading the image
- Set a texture that should be used if an Error occurs while trying to load the image

## There are 3 ways you can use the Picture Downloader, all of them support the core features, but also add new ones
1. Using the Manager:
- You can have as many Picture Donwloaders as you want and use the Manager to well... manage them :D
  - gives you a UI that shows how many Pictures have already been downloaded, how many are still left to download etc.
  - allows to re download all pictures with a button press
  - manager takes control of when to download and to re download them

2. Using the Lite Downloader:
- The Lite Downloader works on it's own and doesn't require a Manager to work, you can still have as many of them as you want though.
  - simpler to setup as no Manager is required
  - still supports all the core Features

3. Using the Picture Loader URL Input + Lite Downloader:
- If you want to be able to enter a URL to load while being in the instance you can use the Picture Loader URL Input. It provides you with an Input field to enter the URL and then just uses the normal Lite Downloader to load the picture.
  - Load URLs from within VRChat
  - Still supports all core features
    - having a default URL to Load On Start also still works
    - *disables Auto Reload on the attached Lite Downloader though

## Which one should I use?
This highly depends on what you want to use them for, but here are a few examples:
- If you want to load a big Gallery of Images / Pictures the Manager would be the best fit, as it takes care of everything at once.
  - Only one Manger per World is supported, but you can have as many downloaders as you want
- If you only want to load a single picture or don't need the Manager you can just use the Lite Downloader.
  - You can have as many Lite Downloaders in a World as you want as they don't interfere with each other.
- If you want to be able to enter a new URL while being in the World you can use the Picture Loader URL Input as it provides you an Input Field and is network synced.
  - You can always only pair one URL Input and one Lite Downloader, you can however have as many parings as you want to.

You can mix an match them as much as you want though. So you could have a Manager taking care of 5 Downloaders, have 3 other Lite Downloaders and 4 Lite Downloaders with URL Input Fields.

### Here is my VRChat World showcasing them:
https://vrchat.com/home/world/wrld_65b1db37-cce6-48d8-b57e-58c3f31b2c93
### Here is a video showcasing it:
https://youtu.be/UT2UuZSvE_4?si=PazJlEoGPF_c3gKy

## Setup:
Take a look at the example scene!
It showcases how all 3 ways can / need to be setup and makes it quite easy to understand.
If you don't know what a setting does / needs just hover over it's name and a tooltip should appear.

1. Manager Setup
- Add the Picture Loader Prefab to your World (Choose between Light & Dark Mode)
- Add the Picture Donwloader script to all the Meshes or Raw Images you want to download pictures for
- Provide a download URL for each Picture Downloader
- Adjust the Settings depending on what you need

2. Lite Setup
- Add the Lite Picture Donwloader script to all the Meshes or Raw Images you want to download pictures for 
- Provide a download URL for the Lite Picture Downloaders
- Adjust the Settings depending on what you need

3. URL Input Setup
- Setup a Lite Picture Downloader as described above
  - You don't need to provide a URl but you can if you want an image to be downloaded by default
  - Auto Reload wont work / will be disabled when using the URL Input
- Add the Picture Loader URL Input Prefab to your World (Choose between Light & Dark Mode)
- Assign the Lite Downloader that you want to use for it

**Tip: If you want the image to look perfect without lighting having any effect on it, I recommend using `VRChat/Sprites/Default` for the Shader.**

**This Asset was made by DrBlackRat**
https://drblackrat.xyz

