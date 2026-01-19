// Keywords Edit Page
const keywordId = window.location.pathname.split('/').pop();

document.addEventListener('DOMContentLoaded', function() {
    loadKeyword();
    setupEventListeners();
});

function setupEventListeners() {
    // Keyword input validation
    const keywordInput = document.getElementById('keyword');
    if (keywordInput) {
        keywordInput.addEventListener('input', function(e) {
            this.value = this.value.toUpperCase().replace(/[^A-Z0-9]/g, '');
        });
    }

    // Auto-response character counter
    const autoResponse = document.getElementById('autoResponse');
    if (autoResponse) {
        autoResponse.addEventListener('input', updateCharCount);
    }

    // Form submission
    const form = document.getElementById('keywordForm');
    if (form) {
        form.addEventListener('submit', handleSubmit);
    }

    // Delete button
    const deleteBtn = document.querySelector('[data-action="delete-keyword"]');
    if (deleteBtn) {
        deleteBtn.addEventListener('click', deleteKeyword);
    }

    // Preview button
    const previewBtn = document.querySelector('[data-action="preview-keyword"]');
    if (previewBtn) {
        previewBtn.addEventListener('click', previewKeyword);
    }
}

async function loadKeyword() {
    try {
        // Mock data for demonstration
        const mockKeyword = {
            id: keywordId,
            keyword: 'JOIN',
            shortCode: '12345',
            campaignId: '1',
            autoResponseMessage: 'Welcome! You\'re now subscribed to our updates. Reply STOP to unsubscribe.',
            requireDoubleOptIn: false,
            trackClicks: true,
            tags: 'promotion, signup',
            isActive: true,
            stats: {
                totalMessages: 1247,
                optIns: 1180,
                optOuts: 67,
                successRate: '94.6%'
            },
            recentActivity: [
                { date: '2024-03-20 14:30', event: 'New opt-in', phone: '+1234567890' },
                { date: '2024-03-20 13:15', event: 'New opt-in', phone: '+1234567891' },
                { date: '2024-03-20 11:45', event: 'Opt-out', phone: '+1234567892' },
                { date: '2024-03-20 10:20', event: 'New opt-in', phone: '+1234567893' },
                { date: '2024-03-19 16:30', event: 'New opt-in', phone: '+1234567894' }
            ]
        };
        
        populateForm(mockKeyword);
        updateStats(mockKeyword.stats);
        renderRecentActivity(mockKeyword.recentActivity);
        
        document.getElementById('loading').style.display = 'none';
        document.getElementById('keywordForm').style.display = 'block';
    } catch (error) {
        document.getElementById('loading').innerHTML = `<div class="alert alert-danger">Error loading keyword: ${error.message}</div>`;
    }
}

function populateForm(keyword) {
    document.getElementById('keyword').value = keyword.keyword;
    document.getElementById('shortCode').value = keyword.shortCode;
    document.getElementById('campaignId').value = keyword.campaignId || '';
    document.getElementById('autoResponse').value = keyword.autoResponseMessage;
    document.getElementById('doubleOptIn').checked = keyword.requireDoubleOptIn;
    document.getElementById('trackClicks').checked = keyword.trackClicks;
    document.getElementById('tags').value = keyword.tags || '';
    document.getElementById('isActive').checked = keyword.isActive;
    
    updateCharCount();
}

function updateStats(stats) {
    document.getElementById('statTotal').textContent = stats.totalMessages.toLocaleString();
    document.getElementById('statOptIns').textContent = stats.optIns.toLocaleString();
    document.getElementById('statOptOuts').textContent = stats.optOuts.toLocaleString();
    document.getElementById('statRate').textContent = stats.successRate;
}

function renderRecentActivity(activities) {
    const container = document.getElementById('recentActivity');
    if (!activities || activities.length === 0) {
        container.innerHTML = '<p class="text-muted small mb-0">No recent activity</p>';
        return;
    }
    
    let html = '<div class="list-group list-group-flush">';
    activities.forEach(activity => {
        const iconClass = activity.event.includes('opt-in') ? 'text-success' : 'text-danger';
        const icon = activity.event.includes('opt-in') ? 'plus-circle' : 'dash-circle';
        html += `
            <div class="list-group-item px-0">
                <div class="d-flex">
                    <i class="bi bi-${icon} ${iconClass} me-2"></i>
                    <div class="flex-grow-1">
                        <small class="d-block">${activity.event}</small>
                        <small class="text-muted">${activity.phone}</small>
                    </div>
                    <small class="text-muted">${activity.date}</small>
                </div>
            </div>
        `;
    });
    html += '</div>';
    container.innerHTML = html;
}

function updateCharCount() {
    const count = document.getElementById('autoResponse').value.length;
    const counter = document.getElementById('charCount');
    counter.textContent = `${count}/160 characters`;
    counter.classList.toggle('text-danger', count > 160);
}

function previewKeyword() {
    const shortCode = document.getElementById('shortCode').value;
    const message = document.getElementById('autoResponse').value;
    
    if (!shortCode || !message) {
        showNotification('Please fill in short code and auto-response message to preview.', 'warning');
        return;
    }
    
    document.getElementById('previewShortCode').textContent = shortCode;
    document.getElementById('previewMessage').textContent = message;
    
    const modal = new bootstrap.Modal(document.getElementById('previewModal'));
    modal.show();
}

async function deleteKeyword() {
    if (!confirm('Are you sure you want to delete this keyword? This action cannot be undone.')) return;
    
    try {
        showNotification('Keyword deleted successfully! (Demo)', 'success');
        setTimeout(() => {
            window.location.href = '/Keywords/Index';
        }, 1000);
    } catch (error) {
        showNotification('Failed to delete keyword: ' + error.message, 'error');
    }
}

async function handleSubmit(e) {
    e.preventDefault();
    
    const autoResponse = document.getElementById('autoResponse').value;
    if (autoResponse.length > 160) {
        showNotification('Auto-response message exceeds 160 characters.', 'error');
        return;
    }
    
    try {
        showNotification('Keyword updated successfully! (Demo)', 'success');
        setTimeout(() => {
            window.location.href = '/Keywords/Index';
        }, 1000);
    } catch (error) {
        showNotification('Failed to update keyword: ' + error.message, 'error');
    }
}
