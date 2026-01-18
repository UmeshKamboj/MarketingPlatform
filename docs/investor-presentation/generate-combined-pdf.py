#!/usr/bin/env python3
"""
Create a comprehensive combined investor package PDF
"""

import os
from pathlib import Path
import markdown2
from weasyprint import HTML, CSS

# Enhanced CSS with better page breaks
CSS_STYLE = """
@page {
    size: Letter;
    margin: 1in;
    @top-right {
        content: "MarketingPlatform - Investor Package";
        font-size: 9pt;
        color: #999;
    }
    @bottom-center {
        content: "Page " counter(page);
        font-size: 9pt;
        color: #999;
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
    font-size: 28pt;
    margin-top: 0;
    margin-bottom: 0.5em;
    border-bottom: 4px solid #0066CC;
    padding-bottom: 0.3em;
    page-break-before: always;
    page-break-after: avoid;
}

h1:first-of-type {
    page-break-before: auto;
}

h2 {
    color: #0066CC;
    font-size: 18pt;
    margin-top: 1.2em;
    margin-bottom: 0.5em;
    border-bottom: 2px solid #0066CC;
    padding-bottom: 0.2em;
    page-break-after: avoid;
}

h3 {
    color: #333;
    font-size: 14pt;
    margin-top: 1em;
    margin-bottom: 0.4em;
    page-break-after: avoid;
}

p, ul, ol, table {
    orphans: 3;
    widows: 3;
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
}

td {
    padding: 0.5em;
    border: 1px solid #ddd;
}

pre {
    background-color: #f4f4f4;
    padding: 1em;
    border-radius: 5px;
    border-left: 4px solid #0066CC;
    overflow-x: auto;
}

code {
    background-color: #f4f4f4;
    padding: 2px 6px;
    border-radius: 3px;
    font-family: 'Courier New', monospace;
}

.section-break {
    page-break-before: always;
    margin-top: 0;
}
"""

def main():
    script_dir = Path(__file__).parent
    pdf_dir = script_dir / "pdfs"
    
    # Priority documents for combined package
    documents = [
        ("README.md", "Overview & Quick Start"),
        ("SLIDES.md", "Investor Pitch Deck"),
        ("FEATURE_SUMMARY.md", "Platform Features"),
        ("BUSINESS_MODEL.md", "Business Model & Financials"),
        ("COMPETITIVE_ANALYSIS.md", "Competitive Analysis"),
        ("TECHNICAL_DEEP_DIVE.md", "Technical Architecture"),
        ("FAQ.md", "Frequently Asked Questions"),
    ]
    
    print("📦 Creating Combined Investor Package PDF...")
    
    # Build combined HTML
    combined_html = """
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset="utf-8">
        <title>MarketingPlatform - Complete Investor Package</title>
    </head>
    <body>
    """
    
    # Add cover page
    combined_html += """
    <div style="text-align: center; padding-top: 3in;">
        <h1 style="font-size: 36pt; border: none; page-break-before: auto;">MarketingPlatform</h1>
        <h2 style="font-size: 24pt; color: #666; border: none;">Complete Investor Package</h2>
        <p style="font-size: 14pt; margin-top: 2em;">SMS, MMS & Email Marketing Platform</p>
        <p style="font-size: 12pt; color: #999; margin-top: 4em;">ASP.NET Core 8.0 | Enterprise-Grade Solution</p>
        <p style="font-size: 12pt; color: #999;">Generated January 2026</p>
    </div>
    <div style="page-break-after: always;"></div>
    """
    
    # Add table of contents
    combined_html += """
    <h1 style="page-break-before: auto;">Table of Contents</h1>
    <ul style="font-size: 12pt; line-height: 2;">
    """
    for i, (filename, title) in enumerate(documents, 1):
        combined_html += f"<li><strong>{i}. {title}</strong></li>"
    combined_html += """
    </ul>
    <div style="page-break-after: always;"></div>
    """
    
    # Add each document
    for i, (filename, title) in enumerate(documents, 1):
        md_file = script_dir / filename
        if md_file.exists():
            try:
                with open(md_file, 'r', encoding='utf-8') as f:
                    md_content = f.read()
                
                html_content = markdown2.markdown(
                    md_content,
                    extras=['fenced-code-blocks', 'tables', 'header-ids', 'task_list', 'strike']
                )
                
                # Add section with page break
                combined_html += f"""
                <div class="section-break">
                    <h1>{i}. {title}</h1>
                    {html_content}
                </div>
                """
                print(f"  ✅ Added: {filename}")
            except Exception as e:
                print(f"  ❌ Error with {filename}: {e}")
        else:
            print(f"  ⚠️  Not found: {filename}")
    
    combined_html += """
    </body>
    </html>
    """
    
    # Generate combined PDF
    output_file = pdf_dir / "MarketingPlatform-Complete-Investor-Package.pdf"
    HTML(string=combined_html).write_pdf(
        output_file,
        stylesheets=[CSS(string=CSS_STYLE)]
    )
    
    # Get file size
    file_size = output_file.stat().st_size
    size_kb = file_size / 1024
    
    print(f"\n{'='*60}")
    print(f"✅ Combined PDF created successfully!")
    print(f"📁 Location: {output_file}")
    print(f"📊 Size: {size_kb:.0f} KB ({file_size:,} bytes)")
    print(f"📄 Pages: ~{len(documents) * 15} pages estimated")
    print(f"{'='*60}")
    print(f"\n📧 Ready to email or share!")

if __name__ == "__main__":
    main()
