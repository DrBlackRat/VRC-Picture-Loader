![PictureLoaderBanner](https://github.com/user-attachments/assets/04264a5f-28cd-4f4d-8b85-4cb2e3b135df)


## Download
The VRC Picture Loader is available via the [Creator Companion](https://vcc.docs.vrchat.com/) as a VPM package!
###  [⬇️ My VPM / Creator Companion Listing](https://vpm.drblackrat.xyz)

For Standalone Unity I still provide a Unity Package with every release.

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
   - Automatically Adjust the Aspect Ratio of UI Raw Images using an Aspect Ratio Filter 
- Set a texture that should be used while loading the image
- Set a texture that should be used if an Error occurs while trying to load the image

## Tablet Downloader
With this Package I also include a Tablet Downloader, it essentially uses a Lite Downloader with a URL Input but on a Tablet that you can carry around!

Here is what it looks like:

<img src="https://github.com/user-attachments/assets/7d5ffe1b-1730-4a48-b14e-8b21a2562f2c" width="300">

It's quite nice to use for showcasing images to others in your instance.
- Supports all URL Input + Lite Downloader Features
- Supports Persistence
- Supports Auto Rotate so your images are always the right way around

You can add it to your Scene by going to the top of the Unity Window under `Tools > Picture Loader` and then clicking on `Add Tablet Downloader Prefab to Scene`.
- There is no limit to how many of these you can have in your World.

To add it to Persistence, just follow the Persistence Setup guide and add the URL Input field to it. You can find it on the Prefab under `Tablet Downloader > Canvas > Mask > UI > Menu > URL Input`.

## There are 4 ways you can use the Picture Downloader, all of them support the core features, but also add new ones
1. Using the Manager:
- You can have as many Picture Downloaders as you want and use the Manager to well... manage them :D
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
  - Is also available as the Tablet Downloader

4. Using Persistence with the Picture Loader URL Input
- Allows you to save the URLs you entered using the Picture Loader URL Input. This allows people to for example decorate their home world with private images. 
  - Any URL the Instance Owner enters will be saved to their Player Object.
  - Once the Instance Owner joins, their images will start to load.
   - This only happens the first time they join.
   - It will overwrite images that other people may have loaded before they 
     joined.
   - Is also available as the Tablet Downloader

## Which one should I use?
This highly depends on what you want to use them for, but here are a few examples:
- If you want to load a big Gallery of Images / Pictures the Manager would be the best fit, as it takes care of everything at once.
  - Only one Manger per World is supported, but you can have as many downloaders as you want
- If you only want to load a single picture or don't need the Manager you can just use the Lite Downloader.
  - You can have as many Lite Downloaders in a World as you want as they don't interfere with each other.
- If you want to be able to enter a new URL while being in the World you can use the Picture Loader URL Input as it provides you an Input Field and is network synced.
  - You can always only pair one URL Input and one Lite Downloader, you can however have as many parings as you want to.
- If you want to be able to save & load pictures in something like a home world, you can use the Picture Loader Persistence as it easily allows you to save & load URLs entered through an Picture Loader URL Input.
  - You can only have one Picture Loader Persistence Prefab per scene, but you can add as many URL Inputs to be saved as you want to.

You can mix an match them as much as you want though. So you could have a Manager taking care of 5 Downloaders, have 3 other Lite Downloaders, 4 Lite Downloaders with URL Input Fields and 6 URL Input Fields with Persistence.

### Here is my VRChat World showcasing them:
https://vrchat.com/home/world/wrld_65b1db37-cce6-48d8-b57e-58c3f31b2c93
### Here is a video showcasing it: (outdated)
https://youtu.be/UT2UuZSvE_4?si=PazJlEoGPF_c3gKy

## Setup:
Take a look at the example scene! You can find it at the top of the Unity Window under `Tools > Picture Loader`.

It showcases how all 4 ways can / need to be setup, as well as the Tablet Downloader and makes it quite easy to understand. If you don't know what a setting does / needs just hover over it's name and a tooltip should appear.

1. Manager Setup
- Add the Picture Loader Prefab to your World (Choose between Light & Dark Mode)
  - you can find these at the top of the Unity Window under `Tools > Picture Loader` 
- Add the Picture Downloader script to all the Meshes or Raw Images you want to download pictures for
- Provide a download URL for each Picture Downloader
- Adjust the Settings depending on what you need

2. Lite Setup
- Add the Lite Picture Downloader script to all the Meshes or Raw Images you want to download pictures for 
- Provide a download URL for the Lite Picture Downloaders
- Adjust the Settings depending on what you need

3. URL Input Setup
- Setup a Lite Picture Downloader as described above
  - You don't need to provide a URl but you can if you want an image to be downloaded by default
  - Auto Reload wont work / will be disabled when using the URL Input
- Add the Picture Loader URL Input Prefab to your World (Choose between Light & Dark Mode)
  - you can find these at the top of the Unity Window under `Tools > Picture Loader` 
- Assign the Lite Downloader that you want to use for it

4. Persistence Setup
- Setup at least one URL Input as described above
- Add the Persistence Prefab to your Scene
  - you can find it at the top of the Unity Window under `Tools > Picture Loader` 
- Add the URL Inputs you want to be saved to the Url Inputs list on the Picture Loader Persistence Prefab

**Tip: If you want the image to look perfect without lighting having any effect on it, I recommend using `VRChat/Sprites/Default` for the Shader.**

**This Asset was made by DrBlackRat**
https://drblackrat.xyz

