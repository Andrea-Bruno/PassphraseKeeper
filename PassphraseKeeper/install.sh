#!/bin/bash
#dont show grub menu
echo "Change GRUB_TIMEOUT=10 to GRUB_TIMEOUT=0"
read -p "Press enter to continue"
nano /etc/default/grub
update-grub
#set automatic login of user
mkdir /etc/systemd/system/getty@tty1.service.d
cd /etc/systemd/system/getty@tty1.service.d
echo "[Service]" > autologin.conf
echo "ExecStart=" >> autologin.conf
echo "ExecStart=-/sbin/agetty -o '-p -f -- \\u' --noclear --autologin user %I \$TERM" >> autologin.conf
#auto start
echo "/usr/share/passphrase-keeper/PassphraseKeeper" > /home/user/.bash_login
#set terminal 246 color
echo  "TERM=xterm-256color" > /etc/environment
#set library libdl.so for console app
ln -s /usr/lib/x86_64-linux-gnu/libdl.so.2 /usr/lib/x86_64-linux-gnu/libdl.so