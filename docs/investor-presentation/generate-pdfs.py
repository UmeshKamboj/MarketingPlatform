#!/usr/bin/env python3
"""
Generate PDF files from investor presentation markdown documents
"""

import os
import sys
from pathlib import Path
import markdown2
from weasyprint import HTML, CSS

# Define professional CSS styling
CSS_STYLE = """
@page {
    size: Letter;
    margin: 1in;
    @top-center {
        content: "MarketingPlatform - Investor Presentation";
        font-size: 10pt;
        color: #666;
    }
    @bottom-center {
        content: "Page " counter(page) " of " counter(pages);
        font-size: 10pt;
        color: #666;
    }
}

body {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    line-height: 1.6;
    color: #333;
    font-size: 11pt;
}

h1 {
    color: #0066CC;
    font-size: 24pt;
    margin-top: 0.5em;
    margin-bottom: 0.5em;
    border-bottom: 3px solid #0066CC;
    padding-bottom: 0.3em;
    page-break-after: avoid;
}

h2 {
    color: #0066CC;
    font-size: 18pt;
    margin-top: 1em;
    margin-bottom: 0.5em;
    border-bottom: 1px solid #0066CC;
    padding-bottom: 0.2em;
    page-break-after: avoid;
}

h3 {
    color: #333;
    font-size: 14pt;
    margin-top: 0.8em;
    margin-bottom: 0.4em;
    page-break-after: avoid;
}

h4 {
    color: #555;
    font-size: 12pt;
    margin-top: 0.6em;
    margin-bottom: 0.3em;
    page-break-after: avoid;
}

p {
    margin-bottom: 0.5em;
    text-align: justify;
}

ul, ol {
    margin-bottom: 0.5em;
    padding-left: 1.5em;
}

li {
    margin-bottom: 0.3em;
}

code {
    background-color: #f4f4f4;
    padding: 2px 6px;
    border-radius: 3px;
    font-family: 'Courier New', monospace;
    font-size: 10pt;
}

pre {
    background-color: #f4f4f4;
    padding: 1em;
    border-radius: 5px;
    overflow-x: auto;
    border-left: 4px solid #0066CC;
}

pre code {
    background-color: transparent;
    padding: 0;
}

table {
    border-collapse: collapse;
    width: 100%;
    margin: 1em 0;
    page-break-inside: avoid;
}

th {
    background-color: #0066CC;
    color: white;
    padding: 0.5em;
    text-align: left;
    border: 1px solid #0066CC;
}

td {
    padding: 0.5em;
    border: 1px solid #ddd;
}

tr:nth-child(even) {
    background-color: #f9f9f9;
}

blockquote {
    border-left: 4px solid #0066CC;
    padding-left: 1em;
    margin-left: 0;
    font-style: italic;
    color: #555;
}

.page-break {
    page-break-after: always;
}

a {
    color: #0066CC;
    text-decoration: none;
}

a:hover {
    text-decoration: underline;
}

strong {
    color: #000;
    font-weight: bold;
}

em {
    font-style: italic;
}

hr {
    border: none;
    border-top: 2px solid #0066CC;
    margin: 2em 0;
}

img {
    max-width: 100%;
    height: auto;
    display: block;
    margin: 1em auto;
}
"""

def convert_markdown_to_pdf(md_file, output_dir):
    """Convert a markdown file to PDF"""
    try:
        # Read markdown file
        with open(md_file, 'r', encoding='utf-8') as f:
            md_content = f.read()
        
        # Convert markdown to HTML
        html_content = markdown2.markdown(
            md_content,
            extras=[
                'fenced-code-blocks',
                'tables',
                'header-ids',
                'task_list',
                'strike',
                'cuddled-lists',
                'footnotes'
            ]
        )
        
        # Wrap in HTML template
        full_html = f"""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="utf-8">
            <title>{md_file.stem} - MarketingPlatform</title>
        </head>
        <body>
            {html_content}
        </body>
        </html>
        """
        
        # Generate PDF
        output_file = output_dir / f"{md_file.stem}.pdf"
        HTML(string=full_html).write_pdf(
            output_file,
            stylesheets=[CSS(string=CSS_STYLE)]
        )
        
        print(f"✅ Generated: {output_file.name}")
        return True
        
    except Exception as e:
        print(f"❌ Error converting {md_file.name}: {str(e)}")
        return False

def main():
    # Get the directory where this script is located
    script_dir = Path(__file__).parent
    
    # Create pdfs output directory
    pdf_dir = script_dir / "pdfs"
    pdf_dir.mkdir(exist_ok=True)
    
    print(f"📄 Converting Markdown to PDF")
    print(f"Source: {script_dir}")
    print(f"Output: {pdf_dir}\n")
    
    # Files to convert (in priority order)
    priority_files = [
        "README.md",
        "SLIDES.md",
        "FEATURE_SUMMARY.md",
        "BUSINESS_MODEL.md",
        "TECHNICAL_DEEP_DIVE.md",
        "COMPETITIVE_ANALYSIS.md",
        "FAQ.md",
        "DEMO_GUIDE.md",
        "SPEAKING_NOTES.md",
        "VIDEO_SCRIPT.md",
        "CONVERSION_GUIDE.md",
        "ASSETS_NEEDED.md"
    ]
    
    success_count = 0
    fail_count = 0
    
    for filename in priority_files:
        md_file = script_dir / filename
        if md_file.exists():
            if convert_markdown_to_pdf(md_file, pdf_dir):
                success_count += 1
            else:
                fail_count += 1
        else:
            print(f"⚠️  File not found: {filename}")
    
    print(f"\n{'='*60}")
    print(f"✅ Successfully converted: {success_count} files")
    if fail_count > 0:
        print(f"❌ Failed: {fail_count} files")
    print(f"📁 PDFs saved to: {pdf_dir}")
    print(f"{'='*60}")

if __name__ == "__main__":
    main()
