const App = {
    setup() {
        const services = {
            wipeDatabase: async (includeDemoData) => {
                try {
                    console.log(AxiosManager);
                    const response = await AxiosManager.post(`/DatabaseCleaner/WipeDatabase?includeDemoData=${includeDemoData}`);
                    return response;
                } catch (error) {
                    throw error;
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
            }
        };

        return {
            handler,
        };
    }

}

Vue.createApp(App).mount('#app');