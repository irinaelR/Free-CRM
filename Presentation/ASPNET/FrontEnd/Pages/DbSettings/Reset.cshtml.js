const App = {
    setup() {
        const state = Vue.reactive({
            fileData: null,
            delimiter: ',',
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
        });

        const fileUploadRef = Vue.ref(null);
        const tableRecordIdRef = Vue.ref(null);

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
                initDropzone();
                await methods.populateRecordMapsListLookup();
                tableRecordListLookup.create();
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
            if (!dropzoneInitialized && fileUploadRef.value) {
                dropzoneInitialized = true;
                const dropzoneInstance = new Dropzone(fileUploadRef.value, {
                    url: "api/FileDocument/UploadDocument",
                    paramName: "file",
                    maxFilesize: 5,
                    acceptedFiles: "text/csv",
                    addRemoveLinks: true,
                    dictDefaultMessage: "Drag and drop a CSV file here to upload",
                    autoProcessQueue: false,
                    init: function () {
                        this.on("addedfile", async function (file) {
                            state.fileData = file;
                            // console.log(state.fileData);
                        });
                    }
                });
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
                        text: 'Form will be closed...',
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
                        text: 'Form will be closed...',
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

                try {
                    const uploadResponse = await services.uploadFile(state.fileData);
                    const fileName = uploadResponse.data?.content?.documentName;

                    const options = {
                        fileName: fileName,
                        delimiter: state.delimiter,
                        dateTimeFormat: state.dateFormat,
                        hasHeaderRecord: true,
                        trimFields: true
                    };
                    const readResponse = await services.processFile(options);
                    const dataToStore = {
                        recordName: state.tableRecordName,
                        data: readResponse.data,
                    };
                    state.uploadedData.push(dataToStore);
                    // console.log(state.uploadedData);

                    state.isSubmitting.upload = false;

                } catch (err) {
                    Swal.fire({
                        icon: 'error',
                        title: state.deleteMode ? 'Delete Failed' : 'Save Failed',
                        text: err.message ?? 'Please check your data.',
                        confirmButtonText: 'Try Again'
                    });

                    state.isSubmitting.upload = false;

                }
            }
        };

        return {
            handler,
            state,
            fileUploadRef,
            tableRecordIdRef,
        };
    }

}

Vue.createApp(App).mount('#app');