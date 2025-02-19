# Download
This release is available via the [Creator Companion](https://vcc.docs.vrchat.com/) as a VPM package!

###  [⬇️ My VPM / Creator Companion Listing](https://vpm.drblackrat.xyz)

# Changes:
This updated massively improves the security of the URL Input!
- implemented OnOwnershipRequest()
  - a player can now only become the Network Owner of a URL Input if (on the current Owners client):
    - it's unlocked
    - they are the Instance Owner
    - they are the Networking Master
  - this *should* prevent client modifcations from overriding the URL
  - added log messages to Ownership Requests
- improved error logging 
