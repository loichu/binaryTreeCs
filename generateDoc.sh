#!/usr/bin/env bash
set -x

if [ -z $(which pandoc) ]; then
    sudo apt install -y pandoc
fi

if [ -z $(which pdflatex) ]; then
    sudo apt install -y texlive-full
fi

if [ -z $(which wkhtmltopdf) ]; then
    sudo apt install -y wkhtmltopdf
fi

mkdir tmp
cd tmp

git clone https://github.com/loichu/binaryTreeCs.wiki.git

pandoc binaryTreeCs.wiki/*.md \
       --toc \
       --highlight-style zenburn \
       -V urlcolor=cyan \
       -V papersize:a4paper \
       -V geometry:margin=1cm \
       -o ../doc.pdf

# Generate PDF from HTML with wkhtmltopdf
pandoc binaryTreeCs.wiki/*.md \
    -s \
    --highlight-style zenburn \
    -t html5 \
    -o ../doc_from_html.pdf
    
cd ..
rm -rf tmp
xpdf doc.pdf
