# How to create a bootable flash drive, as cold storage

## What you need:
1. SD memory of 16 or 32 GB
2. Flash drives, one of which is larger than or equal to the SD
3. PC that must never be connected to the Internet during the operation (disconnect the network cable if present)

## Step by step instructions

1. Prepare a live Debian bootable from Ventoy or Pen drive.
2. Boot the PC with Live Debian and install a Linux without desktop environment on a micro SD
3. Reboot again from the live Debian and use the Disk software to make a .img backup to a drive with sufficient space
4. Now restore the .img file to a good quality flash drive to use for passphrase keeper
5. Use the publish function to create a version of PassphraseKeeper for Linux x64 (it will be created in \PassphraseKeeper\PassphraseKeeper\bin\Release\net6.0\publish\linux-x64)
6. Copy the publish directory to the Ventoy pen drive, and boot the Linux Ventoy pen drive again.
8. Copy the contents of Publish to /usr/share/passphrase-keeper/PassphraseKeeper.

REMINDER ON HOW TO MOUNT A USB FLASH DRIVE:

lsblk
sudo mkdir /mnt/blockstorage
sudo mount /dev/sdb1 /mnt/blockstorage


9. Run install.sh FROM /usr/share/passphrase-keeper/PassphraseKeeper: (sudo chmod +x install.sh; sudo ./install.sh)
10. Passphrase Keeper stand alone is ready, always keep your PC disconnected from the internet if you boot from this flash drive!

 
