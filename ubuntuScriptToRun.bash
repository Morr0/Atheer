#!/bin/bash
apt-get update -y
apt-get upgrade -y
apt install git -y
apt install tmux -y
# Install dotnet - BEGIN
# https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu#2004-
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update -y; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-5.0
sudo apt-get update -y; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y aspnetcore-runtime-5.0
## END
cd /home/ubuntu/
mkdir atheer
cd atheer
git clone https://github.com/Morr0/Atheer .
cd Atheer
# Create a detached tmux session and run the website in it
tmux new-session -d -s server "dotnet run --environment Production"