# Conversion Guide
## Converting Markdown Presentations to PowerPoint, PDF & More

**Version**: 1.0 | **Last Updated**: January 2026

---

## Overview

This guide explains how to convert the MarketingPlatform investor presentation markdown files (especially SLIDES.md) into various formats: PowerPoint, PDF, HTML, and more.

---

## Quick Start

**Recommended Tool**: Marp (Markdown Presentation Ecosystem)

**Install Marp CLI**:
```bash
npm install -g @marp-team/marp-cli
```

**Convert to PowerPoint**:
```bash
marp SLIDES.md --pptx -o MarketingPlatform-Investor-Deck.pptx
```

**Convert to PDF**:
```bash
marp SLIDES.md --pdf -o MarketingPlatform-Investor-Deck.pdf
```

---

## Tool Comparison

| Tool | Best For | Formats | Difficulty |
|------|----------|---------|------------|
| **Marp** | Presentations (our markdown) | PPTX, PDF, HTML | Easy |
| **Pandoc** | Documents, versatile | PPTX, PDF, DOCX, HTML | Medium |
| **reveal.js** | Interactive web presentations | HTML | Medium |
| **mdpdf** | Simple PDF documents | PDF | Easy |
| **Typora** | WYSIWYG editing | PDF, DOCX, HTML | Easy |

**Recommendation**: Use Marp for SLIDES.md. Use Pandoc for other documents.

---

## Option 1: Marp (Recommended for Presentations)

### Installation

**Via npm** (requires Node.js):
```bash
npm install -g @marp-team/marp-cli
```

**Verify Installation**:
```bash
marp --version
```

### Basic Conversion

**PowerPoint**:
```bash
marp SLIDES.md --pptx -o output.pptx
```

**PDF**:
```bash
marp SLIDES.md --pdf -o output.pdf
```

**HTML**:
```bash
marp SLIDES.md --html -o presentation.html
```

### Advanced Options

**With Custom Theme**:
```bash
marp SLIDES.md --theme custom-theme.css --pptx -o output.pptx
```

**Watch Mode** (auto-rebuild on save):
```bash
marp SLIDES.md --watch
```

**All Formats at Once**:
```bash
marp SLIDES.md --pptx --pdf --html -o presentation
# Creates: presentation.pptx, presentation.pdf, presentation.html
```

### Customization

**Custom CSS Theme** (create `custom-theme.css`):
```css
/* @theme custom */
section {
  background-color: #1a1a2e;
  color: #ffffff;
  font-family: 'Arial', sans-serif;
}

h1 {
  color: #00d9ff;
  font-size: 48px;
}

h2 {
  color: #ff6b6b;
  font-size: 36px;
}
```

**Apply Theme**:
```bash
marp SLIDES.md --theme custom-theme.css --pptx
```

---

## Option 2: Pandoc (Universal Converter)

### Installation

**Windows**:
Download installer from https://pandoc.org/installing.html

**Mac**:
```bash
brew install pandoc
```

**Linux (Ubuntu/Debian)**:
```bash
sudo apt-get update
sudo apt-get install pandoc
```

**Verify**:
```bash
pandoc --version
```

### Basic Conversion

**Markdown to PowerPoint**:
```bash
pandoc SLIDES.md -o output.pptx
```

**Markdown to PDF** (requires LaTeX):
```bash
pandoc SLIDES.md -o output.pdf --pdf-engine=xelatex
```

**Markdown to Word**:
```bash
pandoc FEATURE_SUMMARY.md -o feature-summary.docx
```

**Markdown to HTML**:
```bash
pandoc FAQ.md -o faq.html --standalone
```

### Advanced Options

**With Custom Reference PPTX** (for branding):
```bash
pandoc SLIDES.md -o output.pptx --reference-doc=template.pptx
```

Create `template.pptx` with your branding, fonts, colors in PowerPoint first.

**PDF with Table of Contents**:
```bash
pandoc FEATURE_SUMMARY.md -o feature-summary.pdf --toc --toc-depth=2
```

**HTML with CSS Styling**:
```bash
pandoc FAQ.md -o faq.html --standalone --css=styles.css
```

### Troubleshooting

**PDF Errors ("pdflatex not found")**:
Install LaTeX:
- Windows: MiKTeX (https://miktex.org/)
- Mac: MacTeX (`brew install --cask mactex`)
- Linux: `sudo apt-get install texlive-xetex`

**Image Issues**:
Use absolute paths or copy images to same directory as markdown.

---

## Option 3: reveal.js (Interactive Web Presentations)

### Installation

**Via npm**:
```bash
npm install -g reveal-md
```

### Usage

**Start Presentation Server**:
```bash
reveal-md SLIDES.md --theme white
```

Opens browser at http://localhost:1948 with interactive presentation.

**Navigation**:
- Arrow keys: Next/previous slide
- ESC: Slide overview
- S: Speaker notes
- F: Fullscreen

**Export to PDF**:
```bash
reveal-md SLIDES.md --print output.pdf
```

**Export to Static HTML**:
```bash
reveal-md SLIDES.md --static _site
```

Creates directory with self-contained HTML presentation.

### Customization

**Custom Theme**:
```bash
reveal-md SLIDES.md --theme solarized
```

Themes: black, white, league, sky, beige, simple, serif, blood, night, moon, solarized

**Custom CSS**:
```bash
reveal-md SLIDES.md --css custom.css
```

---

## Option 4: mdpdf (Simple PDF Conversion)

### Installation

```bash
npm install -g mdpdf
```

### Usage

**Basic PDF**:
```bash
mdpdf FEATURE_SUMMARY.md
```

**With Custom CSS**:
```bash
mdpdf FEATURE_SUMMARY.md --style=custom.css
```

**All Markdown Files in Directory**:
```bash
mdpdf *.md
```

**Pros**: Simple, fast  
**Cons**: Less control than Pandoc

---

## Option 5: Typora (GUI Editor)

### Installation

Download from https://typora.io/ (Windows, Mac, Linux)

### Usage

1. Open markdown file in Typora
2. File → Export → PDF / Word / HTML
3. Configure options
4. Click Export

**Pros**: WYSIWYG editor, easy for non-technical users  
**Cons**: Not free ($15 one-time purchase)

---

## Batch Conversion Scripts

### Convert All Files to PDF (Bash)

```bash
#!/bin/bash
# convert-all-pdf.sh

for file in *.md; do
  echo "Converting $file..."
  marp "$file" --pdf -o "${file%.md}.pdf"
done

echo "All files converted!"
```

**Usage**:
```bash
chmod +x convert-all-pdf.sh
./convert-all-pdf.sh
```

### Convert Specific Files (PowerShell - Windows)

```powershell
# convert-slides.ps1

$files = @(
  "SLIDES.md",
  "FEATURE_SUMMARY.md",
  "FAQ.md"
)

foreach ($file in $files) {
  $output = $file -replace '\.md$', '.pdf'
  Write-Host "Converting $file to $output..."
  pandoc $file -o $output --pdf-engine=xelatex
}

Write-Host "Conversion complete!"
```

**Usage**:
```powershell
.\convert-slides.ps1
```

---

## Styling & Branding

### Custom PowerPoint Template

1. **Create Template**:
   - Open PowerPoint
   - Design master slides with your branding (logo, colors, fonts)
   - Save as `template.pptx`

2. **Apply Template**:
   ```bash
   pandoc SLIDES.md -o output.pptx --reference-doc=template.pptx
   ```

### Custom PDF Styles

**Create LaTeX Template** (`custom-template.tex`):
```latex
\documentclass{article}
\usepackage{graphicx}
\usepackage[margin=1in]{geometry}
\usepackage{hyperref}

% Custom colors
\definecolor{brandblue}{RGB}{26, 26, 46}

\begin{document}
$body$
\end{document}
```

**Apply Template**:
```bash
pandoc FEATURE_SUMMARY.md -o output.pdf --template=custom-template.tex
```

### Custom HTML/CSS

**Create Stylesheet** (`styles.css`):
```css
body {
  font-family: 'Helvetica Neue', Arial, sans-serif;
  max-width: 900px;
  margin: 0 auto;
  padding: 20px;
  background-color: #f5f5f5;
}

h1 {
  color: #1a1a2e;
  border-bottom: 3px solid #00d9ff;
  padding-bottom: 10px;
}

h2 {
  color: #333;
  margin-top: 30px;
}

code {
  background-color: #e8e8e8;
  padding: 2px 6px;
  border-radius: 3px;
  font-family: 'Courier New', monospace;
}

table {
  width: 100%;
  border-collapse: collapse;
  margin: 20px 0;
}

th, td {
  border: 1px solid #ddd;
  padding: 12px;
  text-align: left;
}

th {
  background-color: #1a1a2e;
  color: white;
}

tr:nth-child(even) {
  background-color: #f9f9f9;
}
```

**Apply Styles**:
```bash
pandoc FAQ.md -o faq.html --standalone --css=styles.css
```

---

## Common Workflows

### Workflow 1: Investor Pitch Deck

**Goal**: Professional PowerPoint for investor meetings

**Steps**:
1. Customize SLIDES.md (update metrics, add company logo)
2. Create branded PowerPoint template
3. Convert with Marp or Pandoc
4. Fine-tune in PowerPoint (add animations if needed)
5. Export to PDF for email distribution

**Commands**:
```bash
# Convert to PPTX
marp SLIDES.md --pptx -o MarketingPlatform-Pitch-Deck.pptx

# Or with custom template
pandoc SLIDES.md -o MarketingPlatform-Pitch-Deck.pptx --reference-doc=template.pptx

# Convert to PDF
marp SLIDES.md --pdf -o MarketingPlatform-Pitch-Deck.pdf
```

---

### Workflow 2: Leave-Behind Document

**Goal**: PDF feature summary for investors

**Steps**:
1. Use FEATURE_SUMMARY.md
2. Convert to professionally styled PDF
3. Include table of contents

**Commands**:
```bash
pandoc FEATURE_SUMMARY.md -o MarketingPlatform-Features.pdf \
  --pdf-engine=xelatex \
  --toc \
  --toc-depth=2 \
  --variable colorlinks=true \
  --variable linkcolor=blue
```

---

### Workflow 3: Interactive Web Demo

**Goal**: Share presentation as website

**Steps**:
1. Convert SLIDES.md to reveal.js presentation
2. Host on GitHub Pages or Netlify
3. Share link with investors

**Commands**:
```bash
# Create static site
reveal-md SLIDES.md --static _site --theme white

# Deploy to GitHub Pages
cd _site
git init
git add .
git commit -m "Add investor presentation"
git remote add origin https://github.com/yourcompany/investor-presentation
git push -u origin main
```

Enable GitHub Pages in repo settings → Pages → Deploy from main branch.

---

## Troubleshooting

### Marp Not Rendering Mermaid Diagrams

**Solution**: Marp doesn't support Mermaid by default. Options:
1. Convert Mermaid to images first (use mermaid-cli)
2. Use reveal.js which supports Mermaid
3. Replace Mermaid with static images in slides

### Pandoc Missing Fonts

**Error**: "Font X not found"

**Solution**:
```bash
# List available fonts
fc-list

# Install missing fonts (Linux)
sudo apt-get install fonts-liberation

# macOS - install via Font Book
# Windows - install .ttf/.otf files
```

### PDF Images Not Showing

**Issue**: Relative image paths broken

**Solutions**:
1. Use absolute paths: `/full/path/to/image.png`
2. Copy images to same directory as markdown
3. Use online images: `https://example.com/image.png`

### PowerPoint Formatting Issues

**Issue**: Slides don't match markdown preview

**Solutions**:
1. Use `--reference-doc` with pre-formatted template
2. Manual cleanup in PowerPoint after conversion
3. Try different conversion tool (Marp vs. Pandoc)

---

## Recommended Setup

**For Best Results**:

1. **Install All Tools**:
   ```bash
   npm install -g @marp-team/marp-cli reveal-md mdpdf
   # Also install Pandoc from website
   ```

2. **Create Conversion Script** (`build.sh`):
   ```bash
   #!/bin/bash
   echo "Building presentation materials..."

   # Slide deck
   marp SLIDES.md --pptx -o output/slides.pptx
   marp SLIDES.md --pdf -o output/slides.pdf

   # Documents
   pandoc FEATURE_SUMMARY.md -o output/features.pdf --toc
   pandoc FAQ.md -o output/faq.pdf --toc
   pandoc TECHNICAL_DEEP_DIVE.md -o output/tech-deep-dive.pdf --toc

   # Web versions
   pandoc README.md -o output/readme.html --standalone --css=styles.css

   echo "Build complete! Check output/ directory"
   ```

3. **Run Build**:
   ```bash
   chmod +x build.sh
   ./build.sh
   ```

---

## Platform-Specific Tips

### Windows

**Install Chocolatey** (package manager):
```powershell
Set-ExecutionPolicy Bypass -Scope Process -Force
iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
```

**Install Tools**:
```powershell
choco install pandoc nodejs
npm install -g @marp-team/marp-cli
```

### macOS

**Install Homebrew** (if not installed):
```bash
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```

**Install Tools**:
```bash
brew install pandoc node
npm install -g @marp-team/marp-cli reveal-md
```

### Linux (Ubuntu/Debian)

```bash
# Install Node.js
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt-get install -y nodejs

# Install Pandoc
sudo apt-get install pandoc

# Install Marp
sudo npm install -g @marp-team/marp-cli reveal-md
```

---

## Final Checklist

Before Sending to Investors:

- [ ] Update all placeholders (company name, metrics, founder names)
- [ ] Add high-quality logo and images
- [ ] Spell-check all documents
- [ ] Convert to PDF and PowerPoint
- [ ] Test presentations on different devices
- [ ] Add password protection if needed (PowerPoint: File → Info → Protect Presentation)
- [ ] Compress files if large (use ZIP)
- [ ] Create compelling email subject and body

---

## Resources

**Official Documentation**:
- Marp: https://marp.app/
- Pandoc: https://pandoc.org/
- reveal.js: https://revealjs.com/

**Tutorials**:
- Marp Tutorial: https://marpit.marp.app/markdown
- Pandoc User Guide: https://pandoc.org/MANUAL.html

**Templates & Themes**:
- Marp Themes: https://github.com/marp-team/marp-core/tree/main/themes
- reveal.js Themes: https://revealjs.com/themes/

---

**Document Version**: 1.0  
**Last Updated**: January 2026

**Questions?** contact@marketingplatform.com
