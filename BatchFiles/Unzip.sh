#!/bin/bash
set -e

if (( $# < 2))
then
    printf "%b" "Error, must supply 2 arguments.\n" >&2
    exit 1
elif
    printf "%b" "Error, must supply 2 arguments.\n" >&2
    exit 2
fi

tar -xf "$1" -C "$2"
exit 0
