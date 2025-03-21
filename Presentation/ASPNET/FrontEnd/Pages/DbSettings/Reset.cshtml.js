const App = {
    setup() {
        const state = Vue.reactive({
            fileData: null,
            delimiter: ',',
            dateFormat: 'yyyy-MM-dd',
        });

        const fileUploadRef = Vue.ref(null);

        Vue.onMounted(async () => {
            Dropzone.autoDiscover = false;
            try {
                initDropzone();

            } catch (e) {
                console.error('page init error:', e);
            } finally {

            }
        });

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
            }
        };

        const handler = {
            handleWipe: async function () {
                try {
                    const response = await services.wipeDatabase(false);
                    Swal.fire({
                        icon: 'success',
                        title: 'Database reset successfull',
                        text: 'Form will be closed...',
                        timer: 2000,
                        showConfirmButton: false
                    });
                } catch (err) {
                    Swal.fire({
                        icon: 'error',
                        title: state.deleteMode ? 'Delete Failed' : 'Save Failed',
                        text: response.data.message ?? 'Please check your data.',
                        confirmButtonText: 'Try Again'
                    });
                }
            },
            handleFileUpload: async function () {
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
                    console.log(readResponse);
                } catch (err) {
                    Swal.fire({
                        icon: 'error',
                        title: state.deleteMode ? 'Delete Failed' : 'Save Failed',
                        text: response.data.message ?? 'Please check your data.',
                        confirmButtonText: 'Try Again'
                    });
                }
            }
        };

        return {
            handler,
            state,
            fileUploadRef,
        };
    }

}

Vue.createApp(App).mount('#app');