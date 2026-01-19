/**
 * Suppression Lists Index Page - DataTables Implementation
 */

let suppressionTable;
let currentFilter = null;
const apiBaseUrl = window.suppressionConfig?.apiBaseUrl || '/api';

$(document).ready(function() {
    initSuppressionTable();
    setupTypeFilter();
});

function initSuppressionTable() {
    suppressionTable = $('#suppressionTable').DataTable({
        serverSide: true,
        processing: true,
        
        ajax: {
            url: apiBaseUrl + '/suppression',
            type: 'POST',
            headers: getAjaxHeaders(),
            data: function(d) {
                return {
                    pageNumber: Math.floor(d.start / d.length) + 1,
                    pageSize: d.length,
                    searchTerm: d.search.value,
                    sortColumn: d.columns[d.order[0]?.column]?.data || 'name',
                    sortDirection: d.order[0]?.dir || 'asc',
                    type: currentFilter
                };
            },
            dataSrc: function(json) {
                json.recordsTotal = json.totalCount || 0;
                json.recordsFiltered = json.totalCount || 0;
                return json.items || json.data?.items || [];
            }
        },
        
        columns: [
            {
                data: 'name',
                name: 'Name',
                orderable: true,
                searchable: true,
                render: function(data, type, row) {
                    if (type === 'display') {
                        const description = row.description ? `<br/><small class="text-muted">${escapeHtml(row.description)}</small>` : '';
                        return `<strong>${escapeHtml(data)}</strong>${description}`;
                    }
                    return data;
                }
            },
            {
                data: 'type',
                name: 'Type',
                orderable: true,
                searchable: false,
                render: function(data) {
                    const typeColors = { 'OptOut': 'danger', 'Bounced': 'warning', 'Complained': 'danger', 'Invalid': 'secondary', 'Custom': 'info' };
                    return createBadge(data, typeColors[data] || 'secondary');
                }
            },
            {
                data: 'entriesCount',
                name: 'Entries',
                orderable: true,
                searchable: false,
                render: function(data) {
                    return formatNumber(data || 0);
                }
            },
            {
                data: 'isActive',
                name: 'Status',
                orderable: true,
                searchable: false,
                render: function(data) {
                    return data ? createBadge('Active', 'success') : createBadge('Inactive', 'secondary');
                }
            },
            {
                data: 'createdAt',
                name: 'Created',
                orderable: true,
                searchable: false,
                render: function(data) {
                    return formatShortDate(data);
                }
            },
            {
                data: 'id',
                name: 'Actions',
                orderable: false,
                searchable: false,
                className: 'no-export text-end',
                render: function(data) {
                    return `
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-primary" onclick="viewEntries(${data})"><i class="bi bi-list-ul"></i></button>
                            <button class="btn btn-outline-secondary" onclick="editList(${data})"><i class="bi bi-pencil"></i></button>
                            <button class="btn btn-outline-info" onclick="exportList(${data})"><i class="bi bi-download"></i></button>
                            <button class="btn btn-outline-danger" onclick="deleteList(${data})"><i class="bi bi-trash"></i></button>
                        </div>
                    `;
                }
            }
        ],
        
        responsive: true,
        pageLength: 25,
        lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
        order: [[4, 'desc']],
        
        dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
             '<"row"<"col-sm-12"Btr>>' +
             '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
        
        buttons: [
            { extend: 'csv', className: 'btn btn-sm btn-outline-primary me-1', text: '<i class="bi bi-file-earmark-csv"></i> CSV', exportOptions: { columns: ':visible:not(.no-export)' } },
            { extend: 'excel', className: 'btn btn-sm btn-outline-success me-1', text: '<i class="bi bi-file-earmark-excel"></i> Excel', exportOptions: { columns: ':visible:not(.no-export)' } },
            { extend: 'pdf', className: 'btn btn-sm btn-outline-danger', text: '<i class="bi bi-file-earmark-pdf"></i> PDF', exportOptions: { columns: ':visible:not(.no-export)' } }
        ],
        
        language: {
            processing: '<div class="spinner-border text-primary"></div>',
            emptyTable: '<div class="text-center py-3"><i class="bi bi-inbox"></i> No suppression lists found</div>',
            info: 'Showing _START_ to _END_ of _TOTAL_ lists'
        },
        
        stateSave: true,
        searchDelay: 300
    });
}

function setupTypeFilter() {
    $('#typeFilter').on('change', function() {
        currentFilter = $(this).val() || null;
        suppressionTable.ajax.reload();
    });
}

function viewEntries(id) { window.location.href = AppUrls.suppression?.entries ? AppUrls.suppression.entries(id) : `/Suppression/Entries/${id}`; }
function editList(id) { window.location.href = AppUrls.suppression?.edit ? AppUrls.suppression.edit(id) : `/Suppression/Edit/${id}`; }

function exportList(id) {
    showNotification('Exporting suppression list...', 'info');
    window.location.href = `${apiBaseUrl}/suppression/${id}/export`;
}

function deleteList(id) {
    confirmAction('Are you sure you want to delete this suppression list?', function() {
        $.ajax({
            url: `${apiBaseUrl}/suppression/${id}`,
            method: 'DELETE',
            headers: getAjaxHeaders(),
            success: function(response) {
                if (response.success) {
                    showNotification('Suppression list deleted successfully!', 'success');
                    suppressionTable.ajax.reload(null, false);
                }
            },
            error: function() {
                showNotification('Failed to delete suppression list', 'error');
            }
        });
    });
}
