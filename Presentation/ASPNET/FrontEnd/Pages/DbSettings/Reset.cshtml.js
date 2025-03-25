
const App = {
    setup() {
        const state = Vue.reactive({
            file1Data: null,
            file2Data: null,
            delimiter: ',',
            decimalDelimiter: ',',
            dateFormat: 'yyyy-MM-dd',
            tableRecordName: '',
            tableRecordListLookupData: [],
            uploadedData: [],
            isSubmitting: {
                reset: false,
                generate: false,
                upload: false,
                confirm: false,
            },
            dropzone: null,
            wrongFileName: '',
        });

        const file1UploadRef = Vue.ref(null);
        const file2UploadRef = Vue.ref(null);
        const tableRecordIdRef = Vue.ref(null);
        const uploadedDataListViewRef = Vue.ref(null);

        const tableRecordListLookup = {
            obj: null,
            create: () => {
                if (state.tableRecordListLookupData.data && Array.isArray(state.tableRecordListLookupData.data)) {
                    tableRecordListLookup.obj = new ej.dropdowns.DropDownList({
                        dataSource: state.tableRecordListLookupData.data,
                        fields: { value: 'name', text: 'name' },
                        placeholder: 'Select the corresponding table',
                        popupHeight: '200px',
                        change: (e) => {
                            state.tableRecordName = e.value;
                        }
                    });
                    tableRecordListLookup.obj.appendTo(tableRecordIdRef.value);
                } else {
                    console.log(state.tableRecordListLookupData.data);
                    console.log(Array.isArray(state.tableRecordListLookupData.data));
                    console.error('Record Maps list lookup data is not available or invalid.');
                }
            },
            refresh: () => {
                if (tableRecordListLookup.obj) {
                    tableRecordListLookup.obj.value = state.tableRecordName;
                }
            },
        };

        Vue.onMounted(async () => {
            Dropzone.autoDiscover = false;
            try {
                state.dropzone = initDropzone();
                // await methods.populateRecordMapsListLookup();
                // tableRecordListLookup.create();
            } catch (e) {
                console.error('page init error:', e);
            } finally {

            }
        });

        Vue.watch(
            () => state.tableRecordName,
            (newVal, oldVal) => {
                tableRecordListLookup.refresh();
            }
        );


        let dropzoneInitialized = false;
        const initDropzone = () => {
            if (!dropzoneInitialized && file1UploadRef.value && file2UploadRef.value) {
                dropzoneInitialized = true;
                const dropzone1Instance = new Dropzone(file1UploadRef.value, {
                    url: "api/FileDocument/UploadDocument",
                    paramName: "file",
                    maxFilesize: 1,
                    acceptedFiles: "text/csv",
                    addRemoveLinks: true,
                    dictDefaultMessage: "Drag and drop a CSV file here to upload the Campaigns",
                    autoProcessQueue: false,
                    init: function () {
                        this.on("addedfile", async function (file) {
                            state.file1Data = file;
                            // console.log(state.fileData);
                        });
                    }
                });
                const dropzone2Instance = new Dropzone(file2UploadRef.value, {
                    url: "api/FileDocument/UploadDocument",
                    paramName: "file",
                    maxFilesize: 1,
                    acceptedFiles: "text/csv",
                    addRemoveLinks: true,
                    dictDefaultMessage: "Drag and drop a CSV file here to upload the Budgets and Expenses",
                    autoProcessQueue: false,
                    init: function () {
                        this.on("addedfile", async function (file) {
                            state.file2Data = file;
                            // console.log(state.fileData);
                        });
                    }
                });
                return [dropzone1Instance, dropzone2Instance];
            }
        };

        const methods = {
            populateRecordMapsListLookup: async function () {
                try {
                    const list = await services.getRecordMaps();
                    state.tableRecordListLookupData = list;
                } catch (err) {
                    throw err;
                }
            },
        }

        const services = {
            wipeDatabase: async (includeDemoData) => {
                try {
                    // console.log(AxiosManager);
                    const response = await AxiosManager.post(`/DatabaseCleaner/WipeDatabase?includeDemoData=${includeDemoData}`);
                    return response;
                } catch (error) {
                    throw error;
                }
            },
            uploadFile: async (file) => {
                const formData = new FormData();
                formData.append('file', file);
                try {
                    const response = await AxiosManager.post('FileDocument/UploadDocument', formData, {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    });
                    return response;
                } catch (err) {
                    throw err;
                }
            },
            processFile: async (options) => {
                try {
                    const response = await AxiosManager.post('Csv/ReadFile', options);
                    return response;
                } catch (err) {
                    throw err;
                }
            },
            getRecordMaps: async () => {
                try {
                    const response = await AxiosManager.get('Csv/GetRecords');
                    return response;
                } catch (err) {
                    throw err;
                }
            },
            reseed: async () => {
                try {
                    const response = await AxiosManager.get('DatabaseCleaner/RepopulateDatabase');
                    return response;
                } catch (err) {
                    throw err;
                }
            },
            saveAll: async () => {
                try {
                    const response = await AxiosManager.post('Csv/Persist', state.uploadedData);
                    return response;
                } catch (err) {
                    throw err;
                }
            }
        };

        const handler = {
            handleWipe: async function () {
                state.isSubmitting.reset = true;
                try {
                    const response = await services.wipeDatabase(false);
                    Swal.fire({
                        icon: 'success',
                        title: 'Database reset successfull',
                        text: 'Message will be closed...',
                        timer: 2000,
                        showConfirmButton: false
                    });
                    state.isSubmitting.reset = false;

                } catch (err) {
                    Swal.fire({
                        icon: 'error',
                        title:  'Database reset failed',
                        text: err.message ?? 'Please check your data.',
                        confirmButtonText: 'Try Again'
                    });
                    state.isSubmitting.reset = false;

                }
            },
            handleReseed: async function () {
                state.isSubmitting.reseed = true;
                try {
                    const response = await services.reseed();
                    Swal.fire({
                        icon: 'success',
                        title: 'Data generated successfully',
                        text: 'Message will be closed...',
                        timer: 2000,
                        showConfirmButton: false
                    });
                    state.isSubmitting.reseed = false;

                } catch (err) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Data generation failed',
                        text: err.message ?? 'Please check your data.',
                        confirmButtonText: 'Try Again'
                    });
                    state.isSubmitting.reseed = false;

                }
            },
            handleFileUpload: async function () {
                state.isSubmitting.upload = true;
                state.wrongFileName = "";
                state.uploadedData = [];
                try {
                    const upload1Response = await services.uploadFile(state.file1Data);
                    const file1Name = upload1Response.data?.content?.documentName;

                    const upload2Response = await services.uploadFile(state.file2Data);
                    const file2Name = upload2Response.data?.content?.documentName;
                    
                    const options = [
                        {
                            fileName: file1Name,
                            fileRealName: state.file1Data.name,
                            delimiter: state.delimiter,
                            dateTimeFormat: state.dateFormat,
                            hasHeaderRecord: true,
                            trimFields: true,
                            tableRecord: state.tableRecordName,
                            decimalSeparator: state.decimalDelimiter,
                        },
                        {
                            fileName: file2Name,
                            fileRealName: state.file2Data.name,
                            delimiter: state.delimiter,
                            dateTimeFormat: state.dateFormat,
                            hasHeaderRecord: true,
                            trimFields: true,
                            tableRecord: state.tableRecordName,
                            decimalSeparator: state.decimalDelimiter,
                        },
                    ];
                    const readResponse = await services.processFile(options);
                    const cmpNb = readResponse.data[0].length;
                    const budNb = readResponse.data[1].length;
                    const expNb = readResponse.data[2].length;

                    Swal.fire({
                        icon: 'success',
                        title: 'Data imported successfully',
                        text: `Campaigns (${cmpNb}), Budgets (${budNb}), Expenses (${expNb})`,
                        showConfirmButton: true
                    });
                    
                    state.fileData = null;
                    if (state.dropzone) {
                        state.dropzone[0].removeAllFiles();
                        state.dropzone[1].removeAllFiles();
                    }
                } catch (err) {
                    const message = err.response ? err.response.data.message : "Something went wrong.";
                    const errorFileName = err.response ? err.response.data.rows.fileName : "unknown file";
                    state.uploadedData = err.response ? err.response.data.rows.rows : [];
                    state.wrongFileName = err.response ? err.response.data.rows.fileName : "unknown file";

                    Swal.fire({
                        icon: 'error',
                        title: `CSV import failed for ${errorFileName}`,
                        text: 'Please check the errors section for more details.',
                        confirmButtonText: 'OK'
                    });
                } finally {
                    state.isSubmitting.upload = false;
                }
            },
            handleSaveAll: async function () {
                state.isSubmitting.confirm = true;
                try {
                    const response = await services.saveAll();
                    state.uploadedData = [];
                    // console.log(response);
;                } catch (err) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Data persistence failed',
                        text: err.message ?? 'Please check your data.',
                        confirmButtonText: 'Try Again'
                    });
                    state.isSubmitting.reseed = false;
                } finally {
                    state.isSubmitting.confirm = false;
                }
            }
        };

        return {
            handler,
            state,
            file1UploadRef,
            file2UploadRef,
            tableRecordIdRef,
        };
    }

}

Vue.createApp(App).mount('#app');