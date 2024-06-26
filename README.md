# Passphrase Keeper
## Trustless application for saving non-custodial wallet passphrases such as Trzeor, Ledger and others

This software, which can be entirely inspected by academics and cryptography experts, allows you to save passphrases for hardware wallets, without having to type them from the keyboard, using a terminal software that can be cold run (disconnected from the internet) by starting it from a live Linux distro, and has been engineered so that, if the save files were to be stolen, it will not be possible to decrypt them with brute force attacks or otherwise: **The security of this software in maintaining the passphrase is greater than that adopted by Ledger or Trezor in keeping it secret from their inside the private key.**

*Characteristics:*
- Resistant to brute force attacks
- Anti keylogger
- Security is in the algorithms and not in the device (inviolable)
- Cold storage

![Passphrase keeper](Screenshot.png?raw=true "Screenshot")

## Are Trezor and Ledger safe?

According to the most modern security concepts, the total security of a device is reduced to the largest flaw; in the case of the most well-known hardware wallets, it is not true, as advertised, that the private key never leaves the device, but leaves it at the moment in which it is written on a piece of paper, this makes events such as:
The cleaning lady finds the passphrase (the 24 words) in a piece of paper hidden between the pages of a book, so this is the security level of this hardware device!
This is the biggest flaw and this is therefore the maximum security level of your wallet.

## Trezor and Ledger bug fix

This project allows you to keep your passphrase in cold storage that cannot be hacked in any way:
We took our idea from the concept of security introduced by Bitcoin technology: Security does not have to be intrinsic in the hardware or in making things inaccessible, but it must be in the algorithm! In contrast to secure backup systems at the hardware level (systems based on trust in the hardware), we have developed a secure save system at the algorithm level (trustless), whose code is open source and inspectable.
What does this mean, if the passphrase saved with our software falls into the hands of hackers or malicious people, it cannot under any circumstances be deciphered, neither with attacks on the encryption algorithms, nor even with a brute force attack!

## Cold passphrase storage
We have created a stand alone version:
A cold storage based on a pen drive bootable system (isolated from the internet), this device will be marketed to finance the project.
We would like to point out that the security of the device is based on algorithms, and working cold is simply additional security:
Any physical or internet theft does not allow the passphrase to be restored!

## FAQ

 *How the passphrase is saved?*
 - The passphrase is not saved: The seed you type when you start the device starts a series of recursive hashes (50 million), the fact that they are recursive operations means that parallelism is not very useful for brute force attacks: Recursion causes so that each hashing operation must wait for the previous result in order to make parallelism useless in brute force attacks. This operation is started simultaneously on eight threads, with a seed derived from the typed seed that is different for each thread, at the end the result of each thread is combined to give a unique result, this result is the 32 byte password that will allow you to encrypt and regenerate passphrase on restore.
   These recursive operations to generate the password take a long time even on powerful computers and cannot be accelerated (due to recursion), and this is the key to resistance to brute force attacks: It would take millions of years!
   In the restore operation, the same recursive hashing operation is performed to regain the password (32 bytes) starting from a typed seed, this means that any typed seed still generates a password which in turn will restore a passphrase, but only the correct seed generates the correct passphrase.
   At a cryptographic level, the passphrase is transformed into a sequence of bytes composed of 11 bits for each word (the minimum necessary), and is compiled with an XOR with the password generated by the recursive hashing operation, so that each bit is changed in a completely random, and this is the data that is saved.
   Upon recovery, any seed typed can restore any passphrase sequence, there is no CRC to indicate which one the user typed, but only the correct seed regenerates the correct passphrase. In addition to this, each application creates a 32-bit entropy which will be different for each device, this entropy is used to encrypt the seed before subjecting it to recursive hashing, this means that it is not possible to carry out a brute force attack in a preventive starting from frequently used words such as seed by doing a recursive pre-hashing of all the most used ones, as each application/device has a different entropy of 32 bytes, making generalized pre-calculations useless.

 *What type of protection do you offer against brute force attacks?*
 - The protection algorithm is published as an open source project: https://github.com/Andrea-Bruno/AntiBruteForce 
   Is a **key derivation function** for defense against brute force attacks.
   This type of defense is based on intensive use of CPU with parallelism, and memory, with optimal robustness to resist GPU and ASIC attacks.
   Brute force protection is characterized by minimalism and ease of understanding, so that there can be no hidden subterfuge, and the scientific community can easily evaluate the open source code by inspecting it.
   Alternatively, brute force protection could also be replaced by older algorithms such as Bcrypt and Argon2 (the latter has also won awards). 

*How can I use this app in isolation (cold storage):*
 - Put the application on a flash drive, ready to boot, and set it as auto-start in the terminal. Before starting, disconnect your PC from the internet (disconnect the ethernet cable), insert the flash drive, and boot by selecting the flash drive as the boot drive (look for your PC's instructions on which button to press at startup to the boot menu appears).
   At this point the application will be launched, isolated from the internet, in a secure environment.

*Is it mandatory to use the app by booting from the flash drive with a PC disconnected from the internet?*
 - Technically, if you're using the original app, no, but you should download it from GitHub in source format and compile it yourself.
   But since it is always best to never trust anything or anyone, your flash drive may not have the original app, so it is highly recommended to boot from the flash drive on a computer disconnected from the internet, physically isolated from the network (unplug the ethernet cable before booting), and uses a boot system based on open source Linux distros and popular sources to make the pen drive bootable.
   You must always consider that someone could replace your pen drive or tamper with the application, so it is important that all save and restore operations take place physically disconnected from the network and the internet, the concept of trustless must also be extended towards the trust of use of the software and the PC (never trust anything or anyone).

*What is the advantage over using any live Linux distro with AES encryption algorithms*
 - You would have the snake biting its own tail situation.
   You would still have the problem of safely saving the AES key, and the best solution would again be Passphrase Keeper.
   The passphrase you save is in all respects representative of the AES 256 key used by bitcoin and other crypto currencies. Passphrase Keeper first transforms the sequence of letters into the relevant private key, and saves it, protecting it from brute force attacks without having to hide it elsewhere a new 32 byte (256 bit) cryptographic key.
   What's the point of protecting a cryptographic key by generating another which then also has to be hidden and protected somewhere? Only the recursive seed algorithm adopted by Passphrase Keeper protects the passphrase from brute force attacks with seeds that can be remembered (they remain in your brain, without the need to be written somewhere because they are too long and difficult to remember).

*Why is XOR used to encrypt the passphrase and not more sophisticated algorithms?*
 - The XOR is an operation without CRC, without mathematical verification, a brute force attack, it will not give any feedback on each attempt regarding the success of the operation. Almost all known systems give feedback when they fail and this helps the trial-and-error search, and they are not designed to hide fixed-length byte arrays, but work in blocks, which means that data as small as a few bytes must still be encapsulated in a block of fixed length, this however helps deduce when the decryption has been successful by observing the bytes between the encrypted ones and the remaining part of the block that has been filled.
   Encrypting a short sequence of bytes using XOR with a random sequence of equal length is a method that gives no clue as to the exodus of the operation when trying to decrypt, and this is exactly what this application needs.
   The random sequence in which the XOR is executed is generated through 8 x 50 million recursive hashing iterations, which means that each single attempt as part of a brute force attack engages a fast computer in the order of minutes.
