# Download
This release is available via the [Creator Companion](https://vcc.docs.vrchat.com/) as a VPM package!

###  [⬇️ My VPM / Creator Companion Listing](https://vpm.drblackrat.xyz)

# Changes:
- Added two new toggles to Persistence:
  - Use Net Master: Allows you to use the Networking Master instead of Instance Owner. With this on Persistence also works in Group and Public Instances.
  - Allow New Mwaster To Save: If the Master changes the new Master will be able to save URL's to their Player Object.

### These changes mean that Persistence can now work in two ways:

#### 1. Instance Owner Mode
  - This is the default behavior
  - Any URL the Instance Owner enters will be saved to their Player Object, even if they rejoin.
  - Once the Instance Owner joins, their images will start to load.
    - This only happens the first time they join.
    - It will overwrite images that other people may have loaded before they joined.

#### 2. Network Master Mode
  - Can be turned on by checking "Use Net Master" on the Persistence Script.
  - Any URL the current Master enters will be saved to their Player Object.
    - If "Allow New Master To Save" is enabled and the Master changes the new Master will now be able to Save URLs.
    - If "Allow New Master To Save" is disabled and the Master changes no one will be able to save URLs in this instance anymore, even if the original Master rejoins.
  - Once the first Master joins, their images will start to load.
    - This only happens once.
        
