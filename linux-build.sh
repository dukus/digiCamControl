# linux build script from source

#!/bin/bash

# DigiCamControl build script for Arch Linux using CMake
# Author: Vortex Viking

# Exit on any error
set -e

# Define variables
REPO_URL="https://github.com/digicamcontrol/digicamcontrol.git"
BUILD_DIR="$HOME/digicamcontrol_build"
INSTALL_DIR="/opt/digicamcontrol"

# Ensure the script is run as root for installation
if [[ $EUID -ne 0 ]]; then
    echo "Please run this script as root (e.g., with sudo)."
    exit 1
fi

# Install dependencies
echo "Installing dependencies..."
pacman -Sy --needed base-devel git mono mono-msbuild nuget cmake

# Clone the repository
if [[ ! -d $BUILD_DIR ]]; then
    echo "Cloning the digiCamControl repository..."
    git clone "$REPO_URL" "$BUILD_DIR"
else
    echo "Repository already exists. Pulling latest changes..."
    cd "$BUILD_DIR"
    git pull
fi

# Navigate to build directory
cd "$BUILD_DIR"

# Create CMake build directory
mkdir -p build && cd build

# Generate build files with CMake
echo "Generating build files using CMake..."
cmake .. -DCMAKE_INSTALL_PREFIX="$INSTALL_DIR"

# Build the project
echo "Building digiCamControl..."
make -j$(nproc)

# Install the software
echo "Installing digiCamControl to $INSTALL_DIR..."
make install

# Final message
echo "digiCamControl has been successfully built and installed!"
echo "Run the application using: mono $INSTALL_DIR/DigiCamControl.exe"
