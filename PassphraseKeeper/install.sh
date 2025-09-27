#!/bin/bash
TARGET_DIR="/usr/share/passphrase-keeper"
SCRIPT_NAME="$(basename "$0")"

# Ensure script is in the target directory
if [ "$(dirname "$(readlink -f "$0")")" != "$TARGET_DIR" ]; then
    echo "Moving script and files to $TARGET_DIR..."
    mkdir -p "$TARGET_DIR"
    cp -r "$(dirname "$(readlink -f "$0")")"/* "$TARGET_DIR"
fi

#don't show grub menu (Change GRUB_TIMEOUT=10 to GRUB_TIMEOUT=0)
export PATH=$PATH:/usr/sbin
sed -i 's/^GRUB_TIMEOUT=.*/GRUB_TIMEOUT=0/' /etc/default/grub
update-grub

# Create user 'user' if it doesn't exist
if ! id -u user >/dev/null 2>&1; then
    echo "User 'user' does not exist. Creating..."
    useradd -m -s /bin/bash user
    echo "User 'user' created with home directory and bash shell."
fi

#set automatic login of user
mkdir /etc/systemd/system/getty@tty1.service.d
cd /etc/systemd/system/getty@tty1.service.d
echo "[Service]" > autologin.conf
echo "ExecStart=" >> autologin.conf
echo "ExecStart=-/sbin/agetty -o '-p -f -- \\u' --noclear --autologin user %I \$TERM" >> autologin.conf

#auto start
chmod +x "$TARGET_DIR/PassphraseKeeper"
echo "$TARGET_DIR/PassphraseKeeper" > "/home/user/.bash_login"

#set terminal 246 color
echo  "TERM=xterm-256color" > /etc/environment

#set library libdl.so for console app
ln -s /usr/lib/x86_64-linux-gnu/libdl.so.2 /usr/lib/x86_64-linux-gnu/libdl.so